using DataAccess.Helpers.Enums;
using DataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_App.ViewModel.Settings;

namespace Social_App.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IUserServices _userServices;
        private readonly IFileServices _fileServices;
        public SettingsController(IUserServices userServices ,IFileServices fileServices)
        {
            _userServices = userServices;
            _fileServices = fileServices;
        }
        public async Task<IActionResult> Index()
        {
            long loggedInUser = 175215637272985601;
            var userDb = await _userServices.GetUser(loggedInUser.ToString());
            return View(userDb);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(ProfilePictureVm profilePictureVm)
        {
            long loggedInUser = 175215637272985601;
            var uploadedAvatarUrl = await _fileServices.UploadFileAsync(profilePictureVm.Avatar ,ImageFileType.profileImage);

            await _userServices.UpdateUserProfile(loggedInUser.ToString(), uploadedAvatarUrl);

            return RedirectToAction("Index");
        }
        [HttpPost]

    public async Task<IActionResult> UpdateProfile(ProfileVm profileVm)
        {
            return RedirectToAction("Index");   
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassWord(UpdatePassWordVM updatePassWordVM)
        {
            return RedirectToAction("Index");
        }
    }
}

