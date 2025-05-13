﻿using DataAccess.Helpers.Constants;
using DataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Social_App.Controllers.Base;


namespace Social_App.Controllers
{
    [Authorize(Roles = $"{AppRoles.User},{AppRoles.Admin}")]
    public class NotificationsController : BaseController
    {
        public readonly INotificationService _notificationService;
        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCount()
        {
            var userId = GetUserId();
            if(!userId.IsNullOrEmpty()) RedirectToLogin();

            var count = await _notificationService.GetUnreadNotificationCountAsync(userId);
            return Json(count);


        }

        public async Task<IActionResult> GetNotifications()
        {
            var userId = GetUserId();
            if (!userId.IsNullOrEmpty()) RedirectToLogin();

            var notifications = await _notificationService.GetNotifications(userId);
            return PartialView("Notifications/_Notifications",notifications);
        }

        [HttpPost]
        public async Task<IActionResult> SetNotificationAsRead(string notificationId)
        {
            var userId = GetUserId();
            if (userId.IsNullOrEmpty()) RedirectToLogin();

            await _notificationService.SetNotificationAsReadÁync(notificationId);

            var notifications = await _notificationService.GetNotifications(userId);

            return PartialView("Notifications/_Notifications", notifications);
        }
    }
}
