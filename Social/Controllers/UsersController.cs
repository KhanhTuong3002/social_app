using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using Social_App.Controllers.Base;

namespace Social_App.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUserServices _userServices;
        public UsersController(IUserServices userServices)
        {
            _userServices = userServices;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(string userid)
        {
           var userPosts = await _userServices.GetUserPosts(userid);
            return View(userPosts);
        }
    }
}
