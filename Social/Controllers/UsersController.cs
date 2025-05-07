using Microsoft.AspNetCore.Mvc;
using Social_App.Controllers.Base;

namespace Social_App.Controllers
{
    public class UsersController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(string userid)
        {
           
            return View();
        }
    }
}
