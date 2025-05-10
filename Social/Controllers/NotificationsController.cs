﻿using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Social_App.Controllers.Base;

namespace Social_App.Controllers
{
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
    }
}
