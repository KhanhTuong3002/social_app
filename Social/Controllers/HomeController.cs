using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social.Models;
using System.Diagnostics;

namespace Social.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SociaDbContex _context;

        public HomeController(ILogger<HomeController> logger , SociaDbContex context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var Allposts = await _context.Posts
                .Include(n => n.user)
                .ToListAsync();
            return View(Allposts);
        }

    }
}
