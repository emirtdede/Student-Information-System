using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Information_System.Data;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace StudentInformationSystem.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var student = await _context.Students
                .Include(s => s.Advisor)
                .FirstOrDefaultAsync(s => s.Id == userId);

            if (student == null) return RedirectToAction("Logout", "Auth");

            return View(student);
        }

        public async Task<IActionResult> Profile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var student = await _context.Students
                .Include(s => s.Advisor)
                .FirstOrDefaultAsync(s => s.Id == userId);

            if (student == null) return RedirectToAction("Logout", "Auth");

            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "Geçersiz dosya." });

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var filePath = Path.Combine(webRootPath, "profile.png");

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string firstName, string lastName, string email)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) 
                return Json(new { success = false, message = "Oturum bulunamadı." });

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == userId);
            if (student == null) 
                return Json(new { success = false, message = "Öğrenci bulunamadı." });

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email))
            {
                return Json(new { success = false, message = "Lütfen tüm alanları doldurunuz." });
            }

            student.FirstName = firstName.Trim();
            student.LastName = lastName.Trim();
            student.Email = email.Trim();

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            // Refresh cookie claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{student.FirstName} {student.LastName}"),
                new Claim(ClaimTypes.Role, "Student")
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return Json(new { success = true, fullName = $"{student.FirstName} {student.LastName}" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var lang = GetRequestLanguage();
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) 
                return Json(new { success = false, message = LocalizationHelper.Get("UserNotFound", lang) });

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == userId);
            if (student == null) 
                return Json(new { success = false, message = LocalizationHelper.Get("UserNotFound", lang) });

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                return Json(new { success = false, message = LocalizationHelper.Get("ErrorOccurred", lang) });
            }

            if (!SecurityHelper.VerifyPassword(student, student.Password, currentPassword))
            {
                return Json(new { success = false, message = LocalizationHelper.Get("CurrentPasswordIncorrect", lang) });
            }

            if (newPassword != confirmPassword)
            {
                return Json(new { success = false, message = LocalizationHelper.Get("PasswordsDoNotMatch", lang) });
            }

            if (newPassword.Length < 6)
            {
                return Json(new { success = false, message = LocalizationHelper.Get("PasswordTooShort", lang) });
            }

            student.Password = SecurityHelper.HashPassword(student, newPassword);
            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = LocalizationHelper.Get("PasswordUpdated", lang) });
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePreferences(bool emailNotificationsEnabled, bool smsNotificationsEnabled, string themePreference, string languagePreference)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) 
                return Json(new { success = false, message = "Oturum bulunamadı." });

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == userId);
            if (student == null) 
                return Json(new { success = false, message = "Öğrenci bulunamadı." });

            student.EmailNotificationsEnabled = emailNotificationsEnabled;
            student.SmsNotificationsEnabled = smsNotificationsEnabled;
            student.ThemePreference = themePreference ?? "light";
            student.LanguagePreference = languagePreference ?? "tr";

            _context.Students.Update(student);
            await _context.SaveChangesAsync();

            // Store the language in cookie to keep it persistent across session/requests
            Response.Cookies.Append("lang", student.LanguagePreference, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = false, // Accessible by JS
                SameSite = SameSiteMode.Lax
            });

            var lang = student.LanguagePreference;
            return Json(new { success = true, message = LocalizationHelper.Get("PreferencesUpdated", lang) });
        }

        private string GetRequestLanguage()
        {
            if (Request.Cookies.TryGetValue("lang", out var lang) && !string.IsNullOrEmpty(lang))
            {
                return lang;
            }
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                var student = _context.Students.Find(userId);
                if (student != null && !string.IsNullOrEmpty(student.LanguagePreference))
                {
                    return student.LanguagePreference;
                }
            }
            var acceptLanguage = Request.Headers["Accept-Language"].ToString();
            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                return acceptLanguage.StartsWith("en", StringComparison.OrdinalIgnoreCase) ? "en" : "tr";
            }
            return "tr";
        }
    }
}
