namespace Student_Information_System.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string TcKimlikNo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Store hashed password in a real app
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Student, Advisor, Admin

        // User Preferences
        public bool EmailNotificationsEnabled { get; set; } = true;
        public bool SmsNotificationsEnabled { get; set; } = false;
        public string ThemePreference { get; set; } = "light";
        public string LanguagePreference { get; set; } = "tr";
    }
}
