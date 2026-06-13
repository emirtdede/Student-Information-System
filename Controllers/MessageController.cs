using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Information_System.Data;
using Student_Information_System.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace StudentInformationSystem.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessageController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Inbox()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var currentUser = await _context.Users.FindAsync(userId);
            if (currentUser == null) return RedirectToAction("Logout", "Auth");

            var receivedMessages = await _context.Messages
                .Include(m => m.Sender)
                .Where(m => m.ReceiverId == userId)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();

            var sentMessages = await _context.Messages
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == userId)
                .OrderByDescending(m => m.SentDate)
                .ToListAsync();

            ViewBag.UserRole = currentUser.Role;

            if (currentUser.Role == "Student")
            {
                var student = await _context.Students
                    .Include(s => s.Advisor)
                    .FirstOrDefaultAsync(s => s.Id == userId);
                ViewBag.Advisor = student?.Advisor;
            }
            else if (currentUser.Role == "Advisor")
            {
                var advisees = await _context.Students
                    .Where(s => s.AdvisorId == userId)
                    .ToListAsync();
                ViewBag.Advisees = advisees;
            }

            var viewModel = new InboxViewModel
            {
                ReceivedMessages = receivedMessages,
                SentMessages = sentMessages
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            var message = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null) return NotFound();

            if (message.SenderId != userId && message.ReceiverId != userId)
            {
                return Forbid();
            }

            if (message.ReceiverId == userId && !message.IsRead)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return View(message);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int receiverId, string subject, string content)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Logout", "Auth");

            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Konu ve içerik alanları boş bırakılamaz.";
                return RedirectToAction("Inbox");
            }

            var message = new Message
            {
                SenderId = userId,
                ReceiverId = receiverId,
                Subject = subject,
                Content = content,
                SentDate = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Mesajınız başarıyla gönderildi.";
            return RedirectToAction("Inbox");
        }
    }

    public class InboxViewModel
    {
        public List<Message> ReceivedMessages { get; set; } = new();
        public List<Message> SentMessages { get; set; } = new();
    }
}
