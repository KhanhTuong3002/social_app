using BussinessObject;
using BussinessObject.Entities;
using DataAccess;
using DataAccess.Helpers;
using DataAccess.Services;
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
        public readonly IPostService _postService;
        
                
        public HomeController(ILogger<HomeController> logger, SociaDbContex context, IPostService postService)
        {
            _logger = logger;
            _context = context;
            _postService = postService;

        }

        public async Task<IActionResult> Index()
        {
            long loggedInUser = 175215637272985601;

            var Allposts = await _postService.GetAllPostsAsync(loggedInUser.ToString());
            return View(Allposts);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            // get the logged user
            long loggedInUser = 175215637272985601;
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
            await _postService.CreatePostAsync(newPost, post.image);

            //find the hashtags in the post content
            var posHashtags = HashtagHelper.GetHashtags(post.content);
            foreach(var hashtag in posHashtags)
            {
                var hashtagDb = await _context.HashTags
                    .Where(h => h.Name == hashtag)
                    .FirstOrDefaultAsync();
                if (hashtagDb != null)
                {
                    hashtagDb.Count += 1;
                    hashtagDb.UpdatedAt = DateTime.UtcNow;

                    _context.HashTags.Update(hashtagDb);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var newHashtag = new HashTag()
                    {
                        Id = SnowflakeGenerator.Generate(),
                        Name = hashtag,
                        Count = 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        PostId = newPost.PostId.ToString(),
                        UserId = loggedInUser.ToString(),
                    };
                    await _context.HashTags.AddAsync(newHashtag);
                    await _context.SaveChangesAsync();
                }
            }

            // redirect to the index page
            return RedirectToAction("Index");
        }
        [HttpPost]

        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
        {
            long loggedInUser = 175215637272985601;
            await _postService.TogglePostLikeAsync(postLikeVM.PostId, loggedInUser.ToString());

            return RedirectToAction("Index");

        }

        [HttpPost]

        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
        {
            long loggedInUser = 175215637272985601;

            await _postService.TogglePostFavoriteAsync(postFavoriteVM.PostId, loggedInUser.ToString());
            return RedirectToAction("Index");

        }

        [HttpPost]

        public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM postVisibilityVM)
        {
            long loggedInUser = 175215637272985601;
            await _postService.TogglePostVisibilityAsync(postVisibilityVM.PostId, loggedInUser.ToString());

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> AddPostComment(PostCommentPM commentVM)
        {
            long loggedInUser = 175215637272985601;
 
            //create a new comment
            var newComment = new Comment()
            {
                Content = commentVM.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PostId = commentVM.PostId,
                UserId = loggedInUser.ToString(),
            };

            await _postService.AddPostCommentAsync(newComment);
            // redirect to the index page
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
        {
            long loggedInUser = 175215637272985601;
            await _postService.ReportPostAsync(postReportVM.PostId, loggedInUser.ToString());

            // redirect to the index page
            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> RemovePostComment(RemoveCommentPM commentVM)
        {
            await _postService.RemovePostCommentAsync(commentVM.CommentId);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> PostDelete (PostDeleteVM postDeleteVM)
        {
            await _postService.RemovePostAsync(postDeleteVM.PostId);

                //update hashtags
/*                var hashtags = HashtagHelper.GetHashtags(postdb.Content);
                foreach (var hashtag in hashtags)
                {
                    var hashtagDb = await _context.HashTags
                        .Where(h => h.Name == hashtag)
                        .FirstOrDefaultAsync();
                    if (hashtagDb != null)
                    {
                        hashtagDb.Count -= 1;
                        hashtagDb.UpdatedAt = DateTime.UtcNow;

                        _context.HashTags.Update(hashtagDb);
                        await _context.SaveChangesAsync();
                    }
             
                }*/

            return RedirectToAction("Index");
        }
    }

}

