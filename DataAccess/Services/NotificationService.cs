using BussinessObject.Entities;
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

        public NotificationService(SociaDbContex sociaDbContex)
        {
            _sociaDbContex = sociaDbContex;
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
