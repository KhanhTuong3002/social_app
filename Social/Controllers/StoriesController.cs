using BussinessObject.Entities;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Social_App.ViewModel.Stories;
using DataAccess.Helpers.Enums;
using Microsoft.AspNetCore.Authorization;
using Social_App.Controllers.Base;
using Microsoft.Extensions.Hosting;
using DataAccess.Helpers.Constants;

namespace Social_App.Controllers
{
    [Authorize(Roles = $"{AppRoles.User},{AppRoles.Admin}")]
    public class StoriesController : BaseController
    {
        public readonly IStoriesServices _storiesServices;
        public readonly IFileServices _fileServices;
        public StoriesController(IStoriesServices storiesServices, IFileServices fileServices)
        {
            _storiesServices = storiesServices;
            _fileServices = fileServices;
        }


        [HttpPost]
        public async Task<IActionResult> CreateStory(StoryVM storyVM)
        {
            var loggedInUser = GetUserId();
            if (loggedInUser == null) return RedirectToLogin();

            var imageUploadPath = await _fileServices.UploadFileAsync(storyVM.Image,ImageFileType.storyImage);
            var videoUploadPath = await _fileServices.UploadFileAsync(storyVM.Video, ImageFileType.StoryVideo);

            // Kiểm tra nếu cả content và image đều rỗng
            if (storyVM.Image == null && storyVM.Video == null)
            {
                // Trả về thông báo lỗi hoặc redirect về trang trước đó
                TempData["ErrorStories"] = "Oh....Failed! You must include an image to create a stories.";
                return RedirectToAction("Index", "Home"); // hoặc trang bạn muốn
            }

            var newStory = new Story()
            {
                ImageUrl = imageUploadPath,
                VideoUrl = videoUploadPath,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                UserId = loggedInUser.ToString(),
            };

            // check if the user uploaded an image
            await _storiesServices.CreateStoryAsync(newStory);

            return RedirectToAction("Index","Home");
        }
    }
}
