using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Information_System.Data;
using Student_Information_System.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentInformationSystem.Controllers
{
    [Authorize]
    public class DiningController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiningController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Logout", "Auth");

            var student = await _context.Students.FindAsync(userId);
            if (student == null) return RedirectToAction("Logout", "Auth");

            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseMeal()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Logout", "Auth");

            var student = await _context.Students.FindAsync(userId);
            if (student == null) return RedirectToAction("Logout", "Auth");

            decimal mealCost = 25.00m; // Fix meal price for students

            if (student.DiningBalance >= mealCost)
            {
                student.DiningBalance -= mealCost;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Afiyet olsun! Yemek fişi alındı. Kalan bakiye: {student.DiningBalance:N2} ₺";
            }
            else
            {
                TempData["ErrorMessage"] = "Yetersiz bakiye! Lütfen Kampüs Kartınıza bakiye yükleyin.";
            }

            return RedirectToAction("Index");
        }
    }
}
