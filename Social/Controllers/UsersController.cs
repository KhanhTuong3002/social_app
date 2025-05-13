using BussinessObject.Entities;
using DataAccess.Helpers.Constants;
using DataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Social_App.Controllers.Base;
using Social_App.ViewModel.Users;

namespace Social_App.Controllers
{
    [Authorize(Roles = $"{AppRoles.User},{AppRoles.Admin}")]
    public class UsersController : BaseController
    {
        private readonly IUserServices _userServices;
        private readonly UserManager<User> _userManager;
        public UsersController(IUserServices userServices, UserManager<User> userManager)
        {
            _userServices = userServices;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);
            var userPosts = await _userServices.GetUserPosts(userid);

            var userPofileVM = new GetUserProfileVM()
            {
                User = user,
                Posts = userPosts
            };
            return View(userPofileVM);
        }    
    }
}
