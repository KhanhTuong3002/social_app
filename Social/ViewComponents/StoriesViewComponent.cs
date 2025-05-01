using DataAccess;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Social_App.ViewComponents
{
    public class StoriesViewComponent : ViewComponent
    {
        public readonly IStoriesServices _storiesServices;
        public StoriesViewComponent(IStoriesServices storiesServices)
        {
            _storiesServices = storiesServices;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
           var allStories = await _storiesServices.GetAllStoriesAsync();
            return View(allStories);
        }
    }
      
}
