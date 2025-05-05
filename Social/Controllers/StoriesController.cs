using BussinessObject.Entities;
using DataAccess;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social_App.ViewModel.Stories;
using DataAccess.Helpers.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Social_App.Controllers
{
    [Authorize]
    public class StoriesController : Controller
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
            long loggedInUser = 175215637272985601;
            
            var imageUploadPath = await _fileServices.UploadFileAsync(storyVM.Image,ImageFileType.storyImage);

            var newStory = new Story()
            {
                ImageUrl = imageUploadPath,
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
