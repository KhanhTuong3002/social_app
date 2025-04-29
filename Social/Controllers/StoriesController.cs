using BussinessObject.Entities;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social_App.ViewModel.Stories;

namespace Social_App.Controllers
{
    public class StoriesController : Controller
    {
        public readonly SociaDbContex _context;
        public StoriesController(SociaDbContex context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var allStories = await _context.Stories.Include(n => n.User).ToListAsync();
                
            return View(allStories);
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
            if (storyVM.Image != null && storyVM.Image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (storyVM.Image.ContentType.Contains("image"))
                {
                    string rootFolderPathImage = Path.Combine(rootFolderPath, "images/stories");
                    Directory.CreateDirectory(rootFolderPathImage); // Create the directory if it doesn't exist

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(storyVM.Image.FileName);
                    string filePath = Path.Combine(rootFolderPathImage, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await storyVM.Image.CopyToAsync(stream);
                    }
                    //set the image url to the new post
                    newStory.ImageUrl = "/images/stories/" + fileName; // Set the image URL to the new post
                }
            }
            await _context.Stories.AddAsync(newStory);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
