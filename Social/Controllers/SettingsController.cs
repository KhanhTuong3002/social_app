using BussinessObject.Entities;
using DataAccess.Helpers.Enums;
using DataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Social_App.Controllers.Base;
using Social_App.ViewModel.Settings;
using System.Security.Claims;

namespace Social_App.Controllers
{
    [Authorize]
    public class SettingsController : BaseController
    {
        private readonly IUserServices _userServices;
        private readonly IFileServices _fileServices;
        private readonly UserManager<User> _userManager;
        public SettingsController(IUserServices userServices ,IFileServices fileServices, UserManager<User> userManager)
        {
            _userServices = userServices;
            _fileServices = fileServices;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userDb = await _userServices.GetUser(loggedInUserId);
            var loggedInUser = await _userManager.GetUserAsync(User);
            return View(userDb);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(ProfilePictureVm profilePictureVm)
        {
            var loggedInUser = GetUserId();
            if (loggedInUser == null) return RedirectToLogin();

            var uploadedAvatarUrl = await _fileServices.UploadFileAsync(profilePictureVm.Avatar ,ImageFileType.profileImage);

            await _userServices.UpdateUserProfile(loggedInUser, uploadedAvatarUrl);

            return RedirectToAction("Index");
        }
    }
}

