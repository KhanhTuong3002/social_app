using DataAccess.Helpers.Constants;
using DataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Social_App.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class AdminController : Controller
    {
        private readonly IAdminServices _adminServices;
        public AdminController(IAdminServices adminServices)
        {
            _adminServices = adminServices;
        }
        [Authorize(Roles = AppRoles.Admin)]
        public IActionResult Index()
        {
            var reportedPosts = _adminServices.GetReportedPostsAsync().Result;

            return View(reportedPosts);
        }
    }
}
