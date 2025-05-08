using BussinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IFriendService
    {
        Task SendRequestAsync(string senderId, string receiverId);

        Task UpdateRequestAsync(string requestId, string status);

        Task RemoveFriendAsync(string friendShipId);

        Task<List<User>> GetSuggestedFriendsAsync(string userId);

    }
}
