using BussinessObject.Entities;
using DataAccess.Dtos;
using DataAccess.Helpers.Constants;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Services
{
    public class FriendService : IFriendService
    {
        private readonly SociaDbContex _sociaDbContex;
        public FriendService(SociaDbContex sociaDbContex)
        {
            _sociaDbContex = sociaDbContex;
        }
        public async Task<List<UserWithFirendCount>> GetSuggestedFriendsAsync(string userId)
        {
           var existingFriendId = await _sociaDbContex.FriendShips
                .Where(f => f.SenderId == userId || f.ReceiverId == userId)
                .Select(f => f.SenderId == userId ? f.ReceiverId : f.SenderId)
                .ToListAsync();

            // peding requests
            var pendingRequestId = await _sociaDbContex.FriendRequests
                .Where(f => (f.SenderId == userId || f.ReceiverId == userId)  && f.Status ==
                FriendShipStatus.Pending)
                .Select(f => f.SenderId == userId ? f.ReceiverId : f.SenderId)
                .ToListAsync();

            //get suggested friends
            var suggestedFriends = await _sociaDbContex.Users
                .Where(u => u.Id != userId && 
                !existingFriendId.Contains(u.Id) && 
                !pendingRequestId.Contains(u.Id))
                .Select(u => new UserWithFirendCount()
                {
                    user = u,
                    FriendCount = _sociaDbContex.FriendShips
                        .Count(f => f.SenderId == u.Id || f.ReceiverId == u.Id)
                })
                .Take(5)
                .ToListAsync();

            return suggestedFriends;

        }
        public async Task UpdateRequestAsync(string requestId, string newStatus)
        {
            var requestdb = await _sociaDbContex.FriendRequests.FirstOrDefaultAsync(n => n.Id == requestId);
            if (requestdb != null)
            {
                requestdb.Status = newStatus;
                requestdb.UpdatedAt = DateTime.UtcNow;
                _sociaDbContex.Update(requestdb);
                await _sociaDbContex.SaveChangesAsync();
            }

            if(newStatus == FriendShipStatus.Accepted)
            {
                var friendShip = new FriendShip
                {
                    SenderId = requestdb.SenderId,
                    ReceiverId = requestdb.ReceiverId,
                    CreatedAt = DateTime.UtcNow
                };
                await _sociaDbContex.FriendShips.AddAsync(friendShip);
                await _sociaDbContex.SaveChangesAsync();
            }
        }
        public async Task SendRequestAsync(string senderId, string receiverId)
        {
            var request = new FriendRequest
            {
                //Id = SnowflakeGenerator.Generate(),
                SenderId = senderId,
                ReceiverId = receiverId,
                CreatedAt = DateTime.UtcNow,
                Status = FriendShipStatus.Pending,
                UpdatedAt = DateTime.UtcNow

            };
            _sociaDbContex.FriendRequests.Add(request);
            await _sociaDbContex.SaveChangesAsync();
        }

        public async Task RemoveFriendAsync(string friendShipId)
        {
           var friendShip = await _sociaDbContex.FriendShips.FirstOrDefaultAsync(n => n.Id == friendShipId);
            if (friendShip != null)
            {
                _sociaDbContex.FriendShips.Remove(friendShip);
                await _sociaDbContex.SaveChangesAsync();
            }
        }

        public async Task<List<FriendRequest>> GetFriendRequestsAsync(string userId)
        {
          var friendRequestsSent = await _sociaDbContex.FriendRequests
                .Include(n => n.sender)
                .Include(n => n.receiver)
                .Where(f => f.SenderId == userId && f.Status == FriendShipStatus.Pending)
                .ToListAsync();

            return friendRequestsSent;
        }

        public async Task<List<FriendRequest>> GetReceivedFriendRequestsAsync(string userId)
        {
            var friendRequestsSent = await _sociaDbContex.FriendRequests
                .Include(n => n.sender)
                .Include(n => n.receiver)
                .Where(f => f.ReceiverId == userId && f.Status == FriendShipStatus.Pending)
                .ToListAsync();

            return friendRequestsSent;
        }
    }
}
