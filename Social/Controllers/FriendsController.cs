using DataAccess.Helpers.Constants;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using Social_App.Controllers.Base;
using Social_App.ViewModel.Friends;

namespace Social_App.Controllers
{
    public class FriendsController : BaseController
    {
        private readonly IFriendService _friendService;
        private readonly INotificationService _notificationService;
        public FriendsController(IFriendService friendService, INotificationService notificationService)
        {
            _friendService = friendService;
            _notificationService = notificationService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) RedirectToLogin();

            var friendsData = new FriendshipVM()
            {
                Friends = await _friendService.GetFriendsAsync(userId),
                FriendRequestSent = await _friendService.GetFriendRequestsAsync(userId),
                FriendRequestReceived = await _friendService.GetReceivedFriendRequestsAsync(userId)

            };

            return View(friendsData);
        }
        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(string receiverId)
        {
            var userId = GetUserId();
            var userName = GetUserFullName();
            if (string.IsNullOrEmpty(userId)) RedirectToLogin();

            await _friendService.SendRequestAsync(userId, receiverId);

            await _notificationService.AddNewNotificationAsync(receiverId, NotificationType.FriendRequest,userName,null);
             
            return RedirectToAction("Index", "Home");
        }
      
        [HttpPost]
        public async Task<IActionResult> UpdateFriendRequest(string requestId,string status)
        {
            var userId = GetUserId();
            var userName = GetUserFullName();
            if (string.IsNullOrEmpty(userId)) RedirectToLogin();

          var request =  await _friendService.UpdateRequestAsync(requestId, status);
             
            if(status == FriendShipStatus.Accepted)
            {
                await _notificationService.AddNewNotificationAsync(request.SenderId, NotificationType.FriendRequestAprroved, userName, null);

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend (string friendshipId)
        {
            await _friendService.RemoveFriendAsync(friendshipId);
            return RedirectToAction("Index");
        }
    }
}
