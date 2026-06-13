using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Information_System.Data;
using Student_Information_System.Models;
using System.Security.Claims;

namespace StudentInformationSystem.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DocumentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var requests = await _context.DocumentRequests
                .Where(d => d.StudentId == userId)
                .OrderByDescending(d => d.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string documentType, string purpose, int copyCount)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            if (string.IsNullOrWhiteSpace(documentType) || string.IsNullOrWhiteSpace(purpose))
            {
                TempData["ErrorMessage"] = "Belge türü ve amaç alanları zorunludur.";
                return RedirectToAction("Index");
            }

            if (copyCount < 1) copyCount = 1;
            if (copyCount > 5) copyCount = 5;

            // Check if there's already a pending request for same document type
            var existingPending = await _context.DocumentRequests
                .AnyAsync(d => d.StudentId == userId && d.DocumentType == documentType && d.Status == "Pending");

            if (existingPending)
            {
                TempData["ErrorMessage"] = "Bu belge türü için zaten bekleyen bir talebiniz var.";
                return RedirectToAction("Index");
            }

            var request = new DocumentRequest
            {
                StudentId = userId,
                DocumentType = documentType,
                Purpose = purpose,
                CopyCount = copyCount,
                Status = "Pending",
                RequestDate = DateTime.Now
            };

            _context.DocumentRequests.Add(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Belge talebiniz başarıyla oluşturuldu. Danışmanınız inceleyecektir.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var request = await _context.DocumentRequests
                .FirstOrDefaultAsync(d => d.Id == id && d.StudentId == userId && d.Status == "Pending");

            if (request != null)
            {
                _context.DocumentRequests.Remove(request);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Belge talebi iptal edildi.";
            }

            return RedirectToAction("Index");
        }
    }
}
