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
                .Include(n => n.user).
                Include(n => n.Likes).ThenInclude(n => n.User)
                .Include(n => n.Comments).ThenInclude(n => n.User)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return View(Allposts);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            // get the logged user
            long loggedInUser = 175060751466102785;
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

            // check if the user uploaded an image
            if (post.image != null && post.image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (post.image.ContentType.Contains("image"))
                {
                    string rootFolderPathImage = Path.Combine(rootFolderPath, "images/Uploaded");
                    Directory.CreateDirectory(rootFolderPathImage); // Create the directory if it doesn't exist

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.image.FileName);
                    string filePath = Path.Combine(rootFolderPathImage, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await post.image.CopyToAsync(stream);
                    }
                    //set the image url to the new post
                    newPost.ImageUrl = "/images/Uploaded/" + fileName; // Set the image URL to the new post
                }
            }
            //add the post to the database
            await _context.Posts.AddAsync(newPost);
            await _context.SaveChangesAsync();

            // redirect to the index page
            return RedirectToAction("Index");
        }
        [HttpPost]

        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
        {
            long loggedInUser = 175060751466102785;

            //check if user liked the post
            var like = await _context.Likes.
                Where(l => l.PostId == postLikeVM.PostId && l.UserId == loggedInUser.ToString()).FirstOrDefaultAsync();

            if(like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newLike = new Like()
                {
                    PostId = postLikeVM.PostId,
                    UserId = loggedInUser.ToString(),
                    CreatedAt = DateTime.UtcNow,
                };
                await _context.Likes.AddAsync(newLike);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");

        }
        [HttpPost]
        public async Task<IActionResult> AddPostComment(PostCommentPM commentVM)
        {
            long loggedInUser = 175060751466102785;

            //create a new comment
         var newComment = new Comment()
         {
             Content = commentVM.Content,
             CreatedAt = DateTime.UtcNow,
             UpdatedAt = DateTime.UtcNow,
             PostId = commentVM.PostId,
             UserId = loggedInUser.ToString(),
         };

            //add the comment to the database
            await _context.Comments.AddAsync(newComment);
            await _context.SaveChangesAsync();

            // redirect to the index page
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> RemovePostComment(RemoveCommentPM commentVM )
        {
            //get the comment
            var commentdb = await _context.Comments.Where(c => c.CommentId == commentVM.CommentId).FirstOrDefaultAsync();
            if (commentdb != null)
            {
                _context.Comments.Remove(commentdb);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
