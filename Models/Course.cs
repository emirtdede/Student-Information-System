using System.Collections.Generic;

namespace Student_Information_System.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty; // e.g. BİL203
        public string Name { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int Ects { get; set; }
        public string Instructor { get; set; } = string.Empty;

        public int? PrerequisiteCourseId { get; set; }
        public Course? PrerequisiteCourse { get; set; }

        public string ClassDay { get; set; } = string.Empty; // e.g. Pazartesi, Salı, Çarşamba, Perşembe, Cuma
        public string StartTime { get; set; } = string.Empty; // e.g. 09:00
        public string EndTime { get; set; } = string.Empty; // e.g. 12:00

        public int Capacity { get; set; } = 30; // Maximum student capacity

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
