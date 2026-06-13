using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Information_System.Data;
using Student_Information_System.Models;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;

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

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return RedirectToAction("Logout", "Auth");

            if (user is Student student)
            {
                await _context.Entry(student).Reference(s => s.Advisor).LoadAsync();
            }

            return View(user);
        }

        public async Task<IActionResult> Profile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return RedirectToAction("Logout", "Auth");

            if (user is Student student)
            {
                await _context.Entry(student).Reference(s => s.Advisor).LoadAsync();
            }

            return View(user);
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

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) 
                return Json(new { success = false, message = "Kullanıcı bulunamadı." });

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email))
            {
                return Json(new { success = false, message = "Lütfen tüm alanları doldurunuz." });
            }

            user.FirstName = firstName.Trim();
            user.LastName = lastName.Trim();
            user.Email = email.Trim();

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Refresh cookie claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return Json(new { success = true, fullName = $"{user.FirstName} {user.LastName}" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var lang = GetRequestLanguage();
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) 
                return Json(new { success = false, message = LocalizationHelper.Get("UserNotFound", lang) });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) 
                return Json(new { success = false, message = LocalizationHelper.Get("UserNotFound", lang) });

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                return Json(new { success = false, message = LocalizationHelper.Get("ErrorOccurred", lang) });
            }

            if (!SecurityHelper.VerifyPassword(user, user.Password, currentPassword))
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

            user.Password = SecurityHelper.HashPassword(user, newPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = LocalizationHelper.Get("PasswordUpdated", lang) });
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePreferences(bool emailNotificationsEnabled, bool smsNotificationsEnabled, string themePreference, string languagePreference)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) 
                return Json(new { success = false, message = "Oturum bulunamadı." });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) 
                return Json(new { success = false, message = "Kullanıcı bulunamadı." });

            user.EmailNotificationsEnabled = emailNotificationsEnabled;
            user.SmsNotificationsEnabled = smsNotificationsEnabled;
            user.ThemePreference = themePreference ?? "light";
            user.LanguagePreference = languagePreference ?? "tr";

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Store the language in cookie to keep it persistent across session/requests
            Response.Cookies.Append("lang", user.LanguagePreference, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = false, // Accessible by JS
                SameSite = SameSiteMode.Lax
            });

            var lang = user.LanguagePreference;
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
                var user = _context.Users.Find(userId);
                if (user != null && !string.IsNullOrEmpty(user.LanguagePreference))
                {
                    return user.LanguagePreference;
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
