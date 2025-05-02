using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;

namespace Social_App.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly IPostService _postService;
        public FavoritesController(IPostService postService)
        {
            _postService = postService;
        }
        public async Task<IActionResult> Index()
        {
            long loggedInUser = 175215637272985601;
            var myFavoritePosts = await _postService.GetAllFavoritePost(loggedInUser.ToString());
            return View(myFavoritePosts);
        }
    }
}
