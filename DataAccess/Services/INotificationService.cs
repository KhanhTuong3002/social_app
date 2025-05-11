using BussinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface INotificationService
    {
        Task AddNewNotificationAsync(string userId, string type , string userfullname, string? postId );
        Task<int> GetUnreadNotificationCountAsync(string userId);
        Task<List<Notification>> GetNotifications(string userId);
    }
}
