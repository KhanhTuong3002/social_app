using BussinessObject;
using BussinessObject.Entities;
using DataAccess.Helpers.Constants;
using DataAccess.Helpers.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class FriendService : IFriendService
    {
        private readonly SociaDbContex _sociaDbContex;
        public FriendService(SociaDbContex sociaDbContex)
        {
            _sociaDbContex = sociaDbContex;
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
    }
}
