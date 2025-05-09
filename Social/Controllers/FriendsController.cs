using DataAccess.Helpers.Constants;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Social_App.Controllers.Base;
using Social_App.ViewModel.Friends;

namespace Social_App.Controllers
{
    public class FriendsController : BaseController
    {
        public readonly IFriendService _friendService;
        public FriendsController(IFriendService friendService)
        {
            _friendService = friendService;
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
            if (string.IsNullOrEmpty(userId)) RedirectToLogin();

            await _friendService.SendRequestAsync(userId, receiverId);
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> CancelFriendRequest(string requestId)
        {
            await _friendService.UpdateRequestAsync(requestId,FriendShipStatus.Canceled);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AcceptFriendRequest(string requestId)
        {
            await _friendService.UpdateRequestAsync(requestId, FriendShipStatus.Accepted);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RejectFriendRequest(string requestId)
        {
            await _friendService.UpdateRequestAsync(requestId, FriendShipStatus.Rejected);
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
