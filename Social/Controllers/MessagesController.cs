using BussinessObject.Entities;
using DataAccess;
using DataAccess.Dtos;
using DataAccess.Helpers.Constants;
using DataAccess.Hubs;
using DataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Social_App.Controllers.Base;

namespace Social_App.Controllers
{
    [Authorize(Roles = $"{AppRoles.User},{AppRoles.Admin}")]
    public class MessagesController : BaseController
    {
        private readonly SociaDbContex _sociaDbContext;
        private readonly IHubContext<ChatHub> _chatHubContext;
        private readonly IFriendService _friendService;

        public MessagesController(
            SociaDbContex sociaDbContext, 
            IHubContext<ChatHub> chatHubContext,
            IFriendService friendService)
        {
            _sociaDbContext = sociaDbContext;
            _chatHubContext = chatHubContext;
            _friendService = friendService;
        }   

        public IActionResult Index(string? userId)
        {
            ViewBag.SelectedUserId = userId;
            return View();
        }

        [HttpGet("messages/history")]
        public async Task<IActionResult> GetMessageHistory(string otherUserId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var messages = await _sociaDbContext.Message
                .Where(m =>
                    (m.SenderId == userId && m.ReceiverId == otherUserId) ||
                    (m.SenderId == otherUserId && m.ReceiverId == userId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            // Mark received messages as read
            var unreadMessages = messages.Where(m => m.ReceiverId == userId && !m.IsRead).ToList();
            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.IsRead = true;
                }
                await _sociaDbContext.SaveChangesAsync();
            }

            return Ok(messages);
        }

        // POST: api/message/send
        [HttpPost("messages/send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var sender = await _sociaDbContext.Users.FindAsync(userId);
            if (sender == null) return NotFound("Sender not found");

            var message = new Message
            {
                SenderId = userId,
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _sociaDbContext.Message.Add(message);
            await _sociaDbContext.SaveChangesAsync();

            // Prepare payload with Sender Details for Toast Notifications and Live Rendering
            var payload = new
            {
                id = message.Id,
                senderId = message.SenderId,
                senderFullName = sender.FullName,
                senderAvatarUrl = string.IsNullOrEmpty(sender.AvatarUrl) ? "/images/avatar/user.png" : sender.AvatarUrl,
                receiverId = message.ReceiverId,
                content = message.Content,
                sentAt = message.SentAt,
                isRead = message.IsRead
            };

            // Broadcast to the Receiver and to other sessions of the Sender
            await _chatHubContext.Clients.User(request.ReceiverId).SendAsync("ReceiveMessage", payload);
            await _chatHubContext.Clients.User(userId).SendAsync("ReceiveMessage", payload);

            return Ok(payload);
        }

        [HttpGet("messages/conversations")]
        public async Task<IActionResult> GetRecentConversations()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var messages = await _sociaDbContext.Message
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();

            var results = messages
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => {
                    var m = g.First();
                    var otherUser = m.SenderId == userId ? m.Receiver : m.Sender;
                    return new
                    {
                        ConversationWithUserId = otherUser?.Id,
                        FullName = otherUser?.FullName,
                        AvatarUrl = string.IsNullOrEmpty(otherUser?.AvatarUrl) ? "/images/avatar/user.png" : otherUser.AvatarUrl,
                        LastMessage = m.Content,
                        SentAt = m.SentAt,
                        IsSender = m.SenderId == userId,
                        IsRead = m.IsRead
                    };
                })
                .OrderByDescending(x => x.SentAt)
                .ToList();

            return Ok(results);
        }

        [HttpGet("messages/friends")]
        public async Task<IActionResult> GetFriends()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var friendships = await _friendService.GetFriendsAsync(userId);
            var friendsList = friendships.Select(f => {
                var otherUser = f.SenderId == userId ? f.Receiver : f.Sender;
                return new
                {
                    Id = otherUser?.Id,
                    FullName = otherUser?.FullName,
                    AvatarUrl = string.IsNullOrEmpty(otherUser?.AvatarUrl) ? "/images/avatar/user.png" : otherUser.AvatarUrl,
                    UserName = otherUser?.UserName
                };
            }).Where(x => x.Id != null).OrderBy(f => f.FullName).ToList();

            return Ok(friendsList);
        }
    }
}
