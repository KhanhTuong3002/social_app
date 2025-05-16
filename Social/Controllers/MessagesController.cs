using BussinessObject.Entities;
using DataAccess;
using DataAccess.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Social_App.Controllers
{
    public class MessagesController : Controller
    {
        private readonly SociaDbContex _sociaDbContext;

        public MessagesController(SociaDbContex sociaDbContext)
        {
            _sociaDbContext = sociaDbContext;
        }   
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetMessageHistory(string user1, string user2)
        {
            var messages = await _sociaDbContext.Message
                .Where(m =>
                    (m.SenderId == user1 && m.ReceiverId == user2) ||
                    (m.SenderId == user2 && m.ReceiverId == user1))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return Ok(messages);
        }

        // POST: api/message/send
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var message = new Message
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                SentAt = DateTime.UtcNow
            };

            _sociaDbContext.Message.Add(message);
            await _sociaDbContext.SaveChangesAsync();

            // SignalR sẽ xử lý việc đẩy tin ở ChatHub
            return Ok(message);
        }
        [HttpGet("conversations/{userId}")]
        public async Task<IActionResult> GetRecentConversations(string userId)
        {
            var recentMessages = await _sociaDbContext.Message
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => g.OrderByDescending(m => m.SentAt).First())
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            var results = recentMessages.Select(m => new
            {
                ConversationWithUserId = m.SenderId == userId ? m.ReceiverId : m.SenderId,
                LastMessage = m.Content,
                SentAt = m.SentAt,
                IsSender = m.SenderId == userId
            });

            return Ok(results);
        }

    }
}
