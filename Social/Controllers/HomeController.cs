using BussinessObject.Entities;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social.Models;
using Social_App.ViewModel.Home;
using System.Diagnostics;

namespace Social.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SociaDbContex _context;

        public HomeController(ILogger<HomeController> logger, SociaDbContex context)
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
        [HttpPost]
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            // get the logged user
            long loggedInUser = 174892253880254465;
            //create a new post
            Post newPost = new Post()
            {
                Content = post.content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ImageUrl = null,
                NrofRepost = 0,
                UserId = loggedInUser.ToString(),
            };
            //add the post to the database
            await _context.Posts.AddAsync(newPost);
            await _context.SaveChangesAsync();

            // redirect to the index page
            return RedirectToAction("Index");
        }
    }
}
