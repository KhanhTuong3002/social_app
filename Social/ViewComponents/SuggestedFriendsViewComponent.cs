using BussinessObject.Entities;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Social_App.ViewComponents
{
    public class SuggestedFriendsViewComponent :ViewComponent
    {
        private readonly IFriendService _friendService;
        public SuggestedFriendsViewComponent(IFriendService friendService)
        {
            _friendService = friendService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loggedInUserId = ((ClaimsPrincipal)User).FindFirstValue(ClaimTypes.NameIdentifier);

            var suggestedFriends = await _friendService.GetSuggestedFriendsAsync(loggedInUserId);
            return View(suggestedFriends);
        }
    }
}
