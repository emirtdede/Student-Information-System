using System.Collections.Generic;

namespace Student_Information_System.Models
{
    public class Student : ApplicationUser
    {
        public string StudentNumber { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int EnrollmentYear { get; set; }
        public double Gpa { get; set; }
        public bool IsInternship1Completed { get; set; } = false;
        public bool IsInternship2Completed { get; set; } = false;
        public bool IsDoubleMajorActive { get; set; } = false;
        public string DoubleMajorDepartment { get; set; } = string.Empty;
        public double DoubleMajorGpa { get; set; }

        public int? AdvisorId { get; set; }
        public Advisor? Advisor { get; set; }

        public decimal TuitionDebt { get; set; } = 0;
        public decimal DiningBalance { get; set; } = 0;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
