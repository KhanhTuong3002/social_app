using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Social_App.ViewComponents
{
    public class StoriesViewComponent : ViewComponent
    {
        public readonly SociaDbContex _context;
        public StoriesViewComponent(SociaDbContex context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allStories = await _context.Stories
                .Where(n => n.CreatedAt >= DateTime.UtcNow.AddHours(-24))
                .Include(n => n.User)
                .ToListAsync();
            return View(allStories);
        }
    }
      
}
