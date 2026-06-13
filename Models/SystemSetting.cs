namespace Student_Information_System.Models
{
    public class SystemSetting
    {
        public int Id { get; set; }
        public bool IsRegistrationActive { get; set; } = true;
        public bool IsGradeEntryActive { get; set; } = true;
        public string ActiveSemester { get; set; } = "2024-2025 Bahar";
    }
}
