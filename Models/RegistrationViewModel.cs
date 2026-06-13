using System.Collections.Generic;

namespace Student_Information_System.Models
{
    public class RegistrationViewModel
    {
        public List<Course> AvailableCourses { get; set; } = new List<Course>();
        public List<Enrollment> MyEnrollments { get; set; } = new List<Enrollment>();
        public Dictionary<int, string> LockedCoursesReason { get; set; } = new Dictionary<int, string>();
        
        public int TotalCredits => MyEnrollments.Sum(e => e.Course.Credits);
        public int TotalEcts => MyEnrollments.Sum(e => e.Course.Ects);
    }
}
