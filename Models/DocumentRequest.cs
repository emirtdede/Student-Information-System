using System;

namespace Student_Information_System.Models
{
    public class DocumentRequest
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        /// <summary>
        /// Belge türü: Transkript, OgrenciBelgesi, IliskiKesmeBelgesi, DisiplinBelgesi, BursYazisi
        /// </summary>
        public string DocumentType { get; set; } = string.Empty;

        /// <summary>
        /// Belgenin hangi amaçla talep edildiği
        /// </summary>
        public string Purpose { get; set; } = string.Empty;

        /// <summary>
        /// Kaç kopya istendiği
        /// </summary>
        public int CopyCount { get; set; } = 1;

        /// <summary>
        /// Durum: Pending, Approved, Rejected, Ready
        /// Ready = belge hazır, öğrenci teslim alabilir
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Danışman/yönetim tarafından eklenen not (red gerekçesi vb.)
        /// </summary>
        public string? AdminNote { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;
        public DateTime? CompletedDate { get; set; }
    }
}
