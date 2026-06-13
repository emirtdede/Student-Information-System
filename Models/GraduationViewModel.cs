using System.Collections.Generic;
using Student_Information_System.Models;

namespace Student_Information_System.Models
{
    public class GraduationViewModel
    {
        public Student Student { get; set; } = null!;
        public int CompletedEcts { get; set; }
        public int RequiredEcts { get; set; } = 240;
        public double CurrentGpa { get; set; }
        public double RequiredGpa { get; set; } = 2.00;
        
        public List<CourseStatusViewModel> CourseStatuses { get; set; } = new();
        
        public bool IsGraduationEligible { get; set; }
    }

    public class CourseStatusViewModel
    {
        public Course Course { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public string Grade { get; set; } = string.Empty;
    }
}
