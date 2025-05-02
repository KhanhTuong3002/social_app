using BussinessObject;
using BussinessObject.Entities;
using DataAccess;
using DataAccess.Helpers;
using DataAccess.Helpers.Enums;
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
       // private readonly SociaDbContex _context;
        public readonly IPostService _postService;
        private readonly IHashtagServices _hashtagServices;
        private readonly IFileServices _fileServices;


        public HomeController(ILogger<HomeController> logger, 
            IPostService postService, IHashtagServices hashtagServices, IFileServices fileServices)
        {
            _logger = logger;
            //_context = context;
            _postService = postService;
            _hashtagServices = hashtagServices;
            _fileServices = fileServices;
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

            var imageUploadPath = await _fileServices.UploadFileAsync(post.image, ImageFileType.postImage);

            //create a new post
            Post newPost = new Post()
            {
                Content = post.content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ImageUrl = imageUploadPath,
                NrofRepost = 0,
                UserId = loggedInUser.ToString(),
            };

            // check if the user uploaded an image
            await _postService.CreatePostAsync(newPost);

            //find the hashtags in the post content
            await _hashtagServices.ProcessHashtagsForNewPostAsync(post.postId, post.content, post.UserId);
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
           var postRemoved = await _postService.RemovePostAsync(postDeleteVM.PostId);
            await _hashtagServices.ProcessHashtagsForRemovePostAsync(postDeleteVM.PostId, postRemoved.Content);


            //update hashtags


            return RedirectToAction("Index");
        }
    }

}

