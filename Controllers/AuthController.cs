using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Information_System.Data;
using Student_Information_System.Models;
using System.Security.Claims;

namespace StudentInformationSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Advisor"))
                {
                    return RedirectToAction("Dashboard", "Advisor");
                }
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string student_id, string password, bool rememberMe = false)
        {
            var lang = GetRequestLanguage();
            if (string.IsNullOrEmpty(student_id))
            {
                ModelState.AddModelError(string.Empty, LocalizationHelper.Get("StudentIdRequired", lang));
                return View();
            }

            string cleanId = student_id.Trim();
            string cleanEmail = cleanId;

            // If it's an email or contains @, handle it
            if (cleanId.Contains("@"))
            {
                cleanId = cleanId.Split('@')[0];
            }
            else
            {
                // If they entered just student number, construct the canonical email as well
                cleanEmail = $"{cleanId}@ogr.oku.edu.tr";
            }

            var user = await _context.Students
                .FirstOrDefaultAsync(u => u.StudentNumber == cleanId || u.Email == cleanEmail || u.Email == student_id);

            if (user != null && SecurityHelper.VerifyPassword(user, user.Password, password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, "Student")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                    ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(14) : null
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Dashboard");
            }

            ModelState.AddModelError(string.Empty, LocalizationHelper.Get("InvalidCredentials", lang));
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Welcome", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string studentId, string email, string password)
        {
            var lang = GetRequestLanguage();
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return Json(new { success = false, message = LocalizationHelper.Get("FillAllFields", lang) });
            }

            if (!email.EndsWith("@ogr.oku.edu.tr"))
            {
                return Json(new { success = false, message = LocalizationHelper.Get("UseInstitutionalEmail", lang) });
            }

            var existing = await _context.Students.AnyAsync(s => s.StudentNumber == studentId || s.Email == email);
            if (existing)
            {
                return Json(new { success = false, message = LocalizationHelper.Get("AlreadyRegistered", lang) });
            }

            var names = fullName.Trim().Split(' ', 2);
            string firstName = names[0];
            string lastName = names.Length > 1 ? names[1] : string.Empty;

            var advisor = await _context.Advisors.FirstOrDefaultAsync();

            var newStudent = new Student
            {
                FirstName = firstName,
                LastName = lastName,
                StudentNumber = studentId,
                Email = email,
                Role = "Student",
                Department = "Computer Engineering",
                EnrollmentYear = DateTime.Now.Year,
                Gpa = 0.0,
                AdvisorId = advisor?.Id,
                TuitionDebt = 0,
                DiningBalance = 0
            };
            newStudent.Password = SecurityHelper.HashPassword(newStudent, password);

            _context.Students.Add(newStudent);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        private string GetRequestLanguage()
        {
            if (Request.Cookies.TryGetValue("lang", out var lang) && !string.IsNullOrEmpty(lang))
            {
                return lang;
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
