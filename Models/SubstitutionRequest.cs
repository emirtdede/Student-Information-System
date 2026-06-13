using System;

namespace Student_Information_System.Models
{
    public class SubstitutionRequest
    {
        public int Id { get; set; }
        
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int OldCourseId { get; set; }
        public Course OldCourse { get; set; } = null!;

        public int NewCourseId { get; set; }
        public Course NewCourse { get; set; } = null!;

        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public DateTime RequestDate { get; set; } = DateTime.Now;
    }
}
