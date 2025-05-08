using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;

namespace Social_App.Controllers
{
    public class FriendsController : Controller
    {
        public readonly IFriendService _friendService;
        public FriendsController(IFriendService friendService)
        {
            _friendService = friendService;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
