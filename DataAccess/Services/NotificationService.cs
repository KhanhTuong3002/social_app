using BussinessObject.Entities;
using DataAccess.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
        public async Task AddNewNotificationAsync(string userId, string message, string type)
        {
            var newNotification = new Notification()
            {
                UserId = userId,
                Message = message,
                IsRead = false,
                Type = type,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _sociaDbContex.Notifications.AddAsync(newNotification);
            await _sociaDbContex.SaveChangesAsync();

            var notificationNumber = await GetUnreadNotificationCountAsync(userId);

            await _hubContext.Clients.User(userId)
                .SendAsync("ReceiveNotification", notificationNumber);
        }

        public async Task<int> GetUnreadNotificationCountAsync(string userId)
        {
           var count = await _sociaDbContex.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
               .CountAsync();

            return count;
        }
    }
}
