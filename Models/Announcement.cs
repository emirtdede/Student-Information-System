using System;

namespace Student_Information_System.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime DatePosted { get; set; } = DateTime.Now;
        public string Category { get; set; } = "General"; // e.g. Academic, General, Urgent
    }
}
