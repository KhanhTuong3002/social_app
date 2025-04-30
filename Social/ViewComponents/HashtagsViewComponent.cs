using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Social_App.ViewComponents
{
    public class HashtagsViewComponent : ViewComponent
    {
        private readonly SociaDbContex _context;
        public HashtagsViewComponent(SociaDbContex context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var oneWeekAgo = DateTime.UtcNow.AddDays(-7);

            var top3Hastags = await _context.HashTags
                .Where(h => h.CreatedAt >= oneWeekAgo)
                .OrderByDescending(n => n.Count)              
                .Take(3)
                .ToListAsync();

            return View(top3Hastags);
        }
    }
}
