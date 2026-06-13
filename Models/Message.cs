using System;

namespace Student_Information_System.Models
{
    public class Message
    {
        public int Id { get; set; }
        
        public int SenderId { get; set; }
        public ApplicationUser Sender { get; set; } = null!;
        
        public int ReceiverId { get; set; }
        public ApplicationUser Receiver { get; set; } = null!;

        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentDate { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
    }
}
