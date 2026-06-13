using System.Collections.Generic;

namespace Student_Information_System.Models
{
    public class Advisor : ApplicationUser
    {
        public string Department { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        // Navigation property for students advised by this advisor
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
