using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Information_System.Data;
using Student_Information_System.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace StudentInformationSystem.Controllers
{
    [Authorize]
    public class SubstitutionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubstitutionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var requests = await _context.SubstitutionRequests
                .Include(r => r.OldCourse)
                .Include(r => r.NewCourse)
                .Where(r => r.StudentId == userId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            var allCourses = await _context.Courses.ToListAsync();
            ViewBag.Courses = allCourses;

            return View(requests);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int oldCourseId, int newCourseId, string reason)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            if (oldCourseId == newCourseId)
            {
                TempData["ErrorMessage"] = "Aynı dersler arasında muafiyet talebi yapılamaz.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["ErrorMessage"] = "Talep gerekçesi boş bırakılamaz.";
                return RedirectToAction("Index");
            }

            var request = new SubstitutionRequest
            {
                StudentId = userId,
                OldCourseId = oldCourseId,
                NewCourseId = newCourseId,
                Reason = reason,
                Status = "Pending",
                RequestDate = DateTime.Now
            };

            _context.SubstitutionRequests.Add(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Muafiyet/İntibak talebiniz danışman onayına gönderilmiştir.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var request = await _context.SubstitutionRequests.FindAsync(id);
            if (request != null && request.StudentId == userId && request.Status == "Pending")
            {
                _context.SubstitutionRequests.Remove(request);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Talep başarıyla iptal edildi.";
            }

            return RedirectToAction("Index");
        }
    }
}
