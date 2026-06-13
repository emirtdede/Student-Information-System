using System.Collections.Generic;

namespace Student_Information_System.Models
{
    public class DashboardViewModel
    {
        public Student Student { get; set; } = null!;
        public List<Announcement> RecentAnnouncements { get; set; } = new List<Announcement>();
        public List<Course> TodayClasses { get; set; } = new List<Course>();
    }
}
