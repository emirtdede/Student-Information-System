namespace Student_Information_System.Models
{
    public class Enrollment
    {
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        
        public double? MidtermGrade { get; set; }
        public double? FinalGrade { get; set; }
        public string LetterGrade { get; set; } = string.Empty;
        public int AbsentCount { get; set; } = 0;
        public bool IsSurveyCompleted { get; set; } = false;
        public string EnrollmentType { get; set; } = "Major"; // Major, DoubleMajor
    }
}
