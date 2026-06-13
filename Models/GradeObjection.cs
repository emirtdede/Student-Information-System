using System;

namespace Student_Information_System.Models
{
    public class GradeObjection
    {
        public int Id { get; set; }
        
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public string ExamType { get; set; } = string.Empty; // Midterm, Final
        public double CurrentGrade { get; set; }
        public double? ProposedGrade { get; set; } // Null until approved/completed
        public string Explanation { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public DateTime RequestDate { get; set; } = DateTime.Now;
    }
}
