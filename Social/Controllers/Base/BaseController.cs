using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Social_App.Controllers.Base
{
    public abstract class BaseController : Controller
    {
       protected string? GetUserId()
        {
            var loggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(loggedInUser))
                return null;
            return (loggedInUser);
        }

        protected string? GetUserFullName()
        {
            var loggedInUserFullname = User.FindFirstValue(ClaimTypes.Name);
            return (loggedInUserFullname);
        }
        protected IActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Authentication");
        }
    }
}
