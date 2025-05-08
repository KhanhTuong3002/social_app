using Microsoft.AspNetCore.Mvc;
namespace Social_App.ViewComponents
{
    public class SuggestedFriendsViewComponent :ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            
            return View();
        }
    }
}
