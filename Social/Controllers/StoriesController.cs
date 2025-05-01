using BussinessObject.Entities;
using DataAccess;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social_App.ViewModel.Stories;

namespace Social_App.Controllers
{
    public class StoriesController : Controller
    {
        public readonly IStoriesServices _storiesServices;
        public StoriesController(IStoriesServices storiesServices)
        {
            _storiesServices = storiesServices;
        }


        [HttpPost]
        public async Task<IActionResult> CreateStory(StoryVM storyVM)
        {
            long loggedInUser = 175215637272985601;

            var newStory = new Story()
            {
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                UserId = loggedInUser.ToString(),
            };

            // check if the user uploaded an image
            await _storiesServices.CreateStoryAsync(newStory, storyVM.Image);

            return RedirectToAction("Index","Home");
        }
    }
}
