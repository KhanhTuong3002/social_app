using BussinessObject.Entities;
using DataAccess.Helpers.Constants;
using DataAccess.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class NotificationService : INotificationService
    {
        private readonly SociaDbContex _sociaDbContex;
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationService(SociaDbContex sociaDbContex, IHubContext<NotificationHub> hubContext)
        {
            _sociaDbContex = sociaDbContex;
            _hubContext = hubContext;
        }
        public async Task AddNewNotificationAsync(string userId, string type, string userfullname, string? postId)
        {
            var newNotification = new Notification()
            {
                UserId = userId,
                Message = GetPostMessage(type, userfullname),
                IsRead = false,
                Type = type,
                PostId = !postId.IsNullOrEmpty() ? postId : null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _sociaDbContex.Notifications.AddAsync(newNotification);
            await _sociaDbContex.SaveChangesAsync();

            var notificationNumber = await GetUnreadNotificationCountAsync(userId);

            await _hubContext.Clients.User(userId)
                .SendAsync("ReceiveNotification", notificationNumber);
        }

        public async Task<List<Notification>> GetNotifications(string userId)
        {
          var allNotifications = await _sociaDbContex.Notifications
                .Where(n => n.UserId == userId)
                .OrderBy(n => n.IsRead)
                .ThenByDescending(n => n.CreatedAt)
                .ToListAsync();

            return allNotifications;
        }

        public async Task<int> GetUnreadNotificationCountAsync(string userId)
        {
           var count = await _sociaDbContex.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
               .CountAsync();

            return count;
        }

        public async Task SetNotificationAsReadÁync(string notificationId)
        {
            var notificationDb = await _sociaDbContex.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notificationDb != null)
            {
                notificationDb.UpdatedAt = DateTime.UtcNow;
                notificationDb.IsRead = true;

                _sociaDbContex.Notifications.Update(notificationDb);
                await _sociaDbContex.SaveChangesAsync();
            }
        }

        private string GetPostMessage(string type, string userfullname)
        {
            var message = "";
            switch (type)
            {
                case NotificationType.Like:
                    message = $"{userfullname} liked your post";
                    break;
                case NotificationType.Comment:
                    message = $"{userfullname} commented on your post";
                    break;
                case NotificationType.Favorite:
                    
                    message = $"{userfullname} favorited your post";
                    break;
                case NotificationType.FriendRequest:
                    message = $"{userfullname} sent you a friend request";
                    break;
                case NotificationType.FriendRequestAprroved:
                    message = $"{userfullname} accepted your friend request";
                    break;
                default:
                    message = "";
                    break;
            }
            return message;
        }
    }
}
