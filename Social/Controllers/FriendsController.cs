﻿using DataAccess.Services;
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
                FriendRequestSent = await _friendService.GetFriendRequestsAsync(userId)
            };

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(string receiverId)
        {
          var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) RedirectToLogin();

            await _friendService.SendRequestAsync(userId, receiverId);
            return RedirectToAction("Index", "Home");
        }
    }
}
