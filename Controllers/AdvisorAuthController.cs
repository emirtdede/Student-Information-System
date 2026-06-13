using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Information_System.Data;
using System.Security.Claims;

namespace StudentInformationSystem.Controllers
{
    public class AdvisorAuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdvisorAuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Student"))
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                return RedirectToAction("Dashboard", "Advisor");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string advisor_id, string password, bool rememberMe = false)
        {
            // Note: advisor_id is matched to TcKimlikNo or a similar unique field for advisors.
            // Using TcKimlikNo here as placeholder for login.
            var user = await _context.Advisors
                .FirstOrDefaultAsync(u => u.TcKimlikNo == advisor_id);

            if (user != null && SecurityHelper.VerifyPassword(user, user.Password, password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, $"{user.Title} {user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, "Advisor")
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

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Welcome", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
