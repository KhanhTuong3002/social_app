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

        public async Task<Story> CreateStoryAsync(Story story)
        {
           
            await _context.Stories.AddAsync(story);
            await _context.SaveChangesAsync();

            return story;
        }

    }
}
