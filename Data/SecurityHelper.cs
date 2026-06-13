using Microsoft.AspNetCore.Identity;
using Student_Information_System.Models;

namespace Student_Information_System.Data
{
    public static class SecurityHelper
    {
        private static readonly PasswordHasher<ApplicationUser> _hasher = new();

        public static string HashPassword(ApplicationUser user, string password)
        {
            return _hasher.HashPassword(user, password);
        }

        public static bool VerifyPassword(ApplicationUser user, string hashedPassword, string password)
        {
            if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(password))
                return false;

            // Support fallback to plaintext passwords (e.g., from initial seed or transition state)
            // ASP.NET Core PasswordHasher hashes usually start with 'AQAAAA' (format V3)
            if (!hashedPassword.StartsWith("AQAAAA") && hashedPassword == password)
            {
                return true;
            }

            try
            {
                var result = _hasher.VerifyHashedPassword(user, hashedPassword, password);
                return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
            }
            catch
            {
                // Fallback in case of verification format exception
                return hashedPassword == password;
            }
        }
    }
}
