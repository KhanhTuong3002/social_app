using BussinessObject.Entities;
using DataAccess.Helpers.Enums;
using DataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_App.Controllers.Base;
using Social_App.ViewModel.Home;
using System.Security.Claims;

namespace Social.Controllers
{
    [Authorize]
    public class HomeController : BaseController
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
            var loggedInUser = GetUserId();
            if (loggedInUser == null) return RedirectToLogin();

            var Allposts = await _postService.GetAllPostsAsync(loggedInUser);
            return View(Allposts);
        }
        public async Task<IActionResult> Details(string postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            return View(post); // It will look for Views/Home/Details.cshtml
        }



        [HttpPost]
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            // get the logged user
            var loggedInUser = GetUserId();
            if (loggedInUser == null) return RedirectToLogin();

            var imageUploadPath = await _fileServices.UploadFileAsync(post.image, ImageFileType.postImage);

            //create a new post
            Post newPost = new Post()
            {
                Content = post.content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ImageUrl = imageUploadPath,
                NrofRepost = 0,
                UserId = loggedInUser,
            };

            // check if the user uploaded an image
            await _postService.CreatePostAsync(newPost);

            //find the hashtags in the post content
            await _hashtagServices.ProcessHashtagsForNewPostAsync(post.postId, post.content, post.UserId);
            // redirect to the index page
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
        {
            var loggedInUser = GetUserId();
            if (loggedInUser == null) return RedirectToLogin();
            await _postService.TogglePostLikeAsync(postLikeVM.PostId, loggedInUser);

            var post = await _postService.GetPostByIdAsync(postLikeVM.PostId);

            return PartialView("Home/_Post", post);

        }

        [HttpPost]

        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
        {
            var loggedInUser = GetUserId();
            if (loggedInUser == null) return RedirectToLogin();

            await _postService.TogglePostFavoriteAsync(postFavoriteVM.PostId, loggedInUser);
            return RedirectToAction("Index");

        }

        [HttpPost]

        public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM postVisibilityVM)
        {
            var loggedInUser = GetUserId();
            if (loggedInUser == null) return RedirectToLogin();
            await _postService.TogglePostVisibilityAsync(postVisibilityVM.PostId, loggedInUser);

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> AddPostComment(PostCommentPM commentVM)
        {
            var loggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //create a new comment
            var newComment = new Comment()
            {
                Content = commentVM.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PostId = commentVM.PostId,
                UserId = loggedInUser,
            };

            await _postService.AddPostCommentAsync(newComment);
            // redirect to the index page
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
        {
            var loggedInUser = GetUserId();
            if (loggedInUser == null) return RedirectToLogin();
            await _postService.ReportPostAsync(postReportVM.PostId, loggedInUser);

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

