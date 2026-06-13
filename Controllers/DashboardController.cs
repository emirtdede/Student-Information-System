using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Information_System.Data;
using Student_Information_System.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace StudentInformationSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Logout", "Auth");
            }

            var student = await _context.Students
                .Include(s => s.Advisor)
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == userId);

            if (student == null)
            {
                // Might be an advisor
                if (User.IsInRole("Advisor"))
                {
                    return RedirectToAction("Dashboard", "Advisor");
                }
                return RedirectToAction("Logout", "Auth");
            }

            var announcements = await _context.Announcements
                .OrderByDescending(a => a.DatePosted)
                .Take(5)
                .ToListAsync();

            var todayClasses = student.Enrollments
                .Where(e => e.Status == "Approved")
                .Select(e => e.Course)
                .ToList();

            var viewModel = new DashboardViewModel
            {
                Student = student,
                RecentAnnouncements = announcements,
                TodayClasses = todayClasses
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Notifications()
        {
            var announcements = await _context.Announcements
                .OrderByDescending(a => a.DatePosted)
                .ToListAsync();
            return View(announcements);
        }

        [HttpPost]
        public async Task<IActionResult> AddDiningBalance(decimal amount)
        {
            if (amount <= 0) return RedirectToAction("Index");

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var student = await _context.Students.FindAsync(userId);
            if (student != null)
            {
                student.DiningBalance += amount;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"{amount:N2} TL bakiye başarıyla yüklendi.";
            }

            return RedirectToAction("Index");
        }
    }
}
