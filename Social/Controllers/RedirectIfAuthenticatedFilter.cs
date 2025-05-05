using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

public class RedirectIfAuthenticatedAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            // Nếu đã đăng nhập, chuyển hướng về Home/Index
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}
