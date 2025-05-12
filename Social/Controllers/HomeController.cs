using BussinessObject;
using BussinessObject.Entities;
using DataAccess.Helpers.Constants;
using DataAccess.Helpers.Enums;
using DataAccess.Hubs;
using DataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        private readonly INotificationService _notificationService;


        public HomeController(ILogger<HomeController> logger, 
            IPostService postService, IHashtagServices hashtagServices, IFileServices fileServices,
            IHubContext<NotificationHub> hubContext, INotificationService notificationService) 
        {
            _logger = logger;
            //_context = context;
            _postService = postService;
            _hashtagServices = hashtagServices;
            _fileServices = fileServices;
            _notificationService = notificationService;
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
            return View(post); 
        }



        [HttpPost]
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            // get the logged user
            var loggedInUser = GetUserId();
            if (loggedInUser == null) return RedirectToLogin();

            var imageUploadPath = await _fileServices.UploadFileAsync(post.image, ImageFileType.postImage);

            // Kiểm tra nếu cả content và image đều rỗng
            if (string.IsNullOrWhiteSpace(post.content) && post.image == null)
            {
                // Trả về thông báo lỗi hoặc redirect về trang trước đó
                TempData["Error"] = "Oh....Failed! You must include either content or an image to create a post.";
                return RedirectToAction("Index"); // hoặc trang bạn muốn
            }

            //create a new post
            Post newPost = new Post()
            {
                PostId = SnowflakeGenerator.Generate(),
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
            if (!string.IsNullOrWhiteSpace(post.content))
            {
                await _hashtagServices.ProcessHashtagsForNewPostAsync(newPost.PostId, post.content, newPost.UserId);
            }
            // redirect to the index page
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
        {
            var loggedInUser = GetUserId();
            var UserName = GetUserFullName();
            if (loggedInUser == null) return RedirectToLogin();

          var result =  await _postService.TogglePostLikeAsync(postLikeVM.PostId, loggedInUser);          
          var post = await _postService.GetPostByIdAsync(postLikeVM.PostId);

            if (result.SendNotification && loggedInUser != post.UserId)
                await _notificationService.AddNewNotificationAsync
                    (post.UserId,NotificationType.Like , UserName, postLikeVM.PostId);

            return PartialView("Home/_Post",post); // mất layout trên details/favorite 


        }

        [HttpPost]

        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM postFavoriteVM)
        {
            var loggedInUser = GetUserId();
            var UserName = GetUserFullName();
            if (loggedInUser == null) return RedirectToLogin();

           var result = await _postService.TogglePostFavoriteAsync(postFavoriteVM.PostId, loggedInUser);

            var post = await _postService.GetPostByIdAsync(postFavoriteVM.PostId);

            if (result.SendNotification && loggedInUser != post.UserId)
                await _notificationService.AddNewNotificationAsync
                    (post.UserId, NotificationType.Favorite, UserName, postFavoriteVM.PostId);

            return PartialView("Home/_Post", post); // mất layout trên details/favorite 

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
            var UserName = GetUserFullName();

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

            var post = await _postService.GetPostByIdAsync(commentVM.PostId);

            if (loggedInUser != post.UserId)
                await _notificationService.AddNewNotificationAsync
                    (post.UserId, NotificationType.Comment, UserName, commentVM.PostId);
            // redirect to the index page
            return PartialView("Home/_Post", post); // mất layout trên details/favorite 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
        {
            var loggedInUser = GetUserId();
            if (loggedInUser == null) return RedirectToLogin();
            await _postService.ReportPostAsync(postReportVM.PostId, loggedInUser);

            // redirect to the index page
            return RedirectToAction("Index");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePostComment(RemoveCommentPM commentVM)
        {
            await _postService.RemovePostCommentAsync(commentVM.CommentId);
            var post = await _postService.GetPostByIdAsync(commentVM.PostId);
            return PartialView("Home/_Post", post); // mất layout trên details/favorite 
        }



        public async Task<IActionResult> PostDelete (PostDeleteVM postDeleteVM)
        {
           var postRemoved = await _postService.RemovePostAsync(postDeleteVM.PostId);
            await _hashtagServices.ProcessHashtagsForRemovePostAsync(postDeleteVM.PostId, postRemoved.Content);

            return RedirectToAction("Index");
        }
    }

}

