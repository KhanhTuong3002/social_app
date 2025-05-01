using BussinessObject.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class StoriesServices : IStoriesServices
    {
        private readonly SociaDbContex _context;
        public StoriesServices(SociaDbContex context)
        {
            _context = context;
        }

        public async Task<List<Story>> GetAllStoriesAsync()
        {
            var allStories = await _context.Stories
               .Where(n => n.CreatedAt >= DateTime.UtcNow.AddHours(-24))
               .Include(n => n.User)
               .ToListAsync();

            return allStories;
        }

        public async Task<Story> CreateStoryAsync(Story story, IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (image.ContentType.Contains("image"))
                {
                    string rootFolderPathImage = Path.Combine(rootFolderPath, "images/stories");
                    Directory.CreateDirectory(rootFolderPathImage); // Create the directory if it doesn't exist

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string filePath = Path.Combine(rootFolderPathImage, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    //set the image url to the new post
                    story.ImageUrl = "/images/stories/" + fileName; // Set the image URL to the new post
                }
            }
            await _context.Stories.AddAsync(story);
            await _context.SaveChangesAsync();

            return story;
        }

    }
}
