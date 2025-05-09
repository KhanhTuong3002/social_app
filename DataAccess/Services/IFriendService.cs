using BussinessObject.Entities;
using DataAccess.Dtos;
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

        Task<List<UserWithFirendCount>> GetSuggestedFriendsAsync(string userId);
        Task<List<FriendRequest>> GetFriendRequestsAsync(string userId);
        Task<List<FriendRequest>> GetReceivedFriendRequestsAsync(string userId);
        Task<List<FriendShip>> GetFriendsAsync(string userId);

    }
}
