using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            // Gửi cho user có connectionId đã được gán với receiverId
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message);
        }
        public override async Task OnConnectedAsync()
        {
            string? userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                // Log, hoặc xử lý gì đó nếu cần
                await Groups.AddToGroupAsync(Context.ConnectionId, userId); // Không bắt buộc nhưng hữu ích
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string? userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
