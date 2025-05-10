using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface INotificationService
    {
        Task AddNewNotificationAsync(string userId, string message, string type);
    }
}
