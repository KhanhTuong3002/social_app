using BussinessObject;
using BussinessObject.Entities;
using DataAccess.Helpers.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Social_App.ViewModel.Authentication;
using System.Security.Claims;
using Social_App.ViewModel.Settings;

namespace Social_App.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AuthenticationController(UserManager<User> userManager , SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [RedirectIfAuthenticated]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);
            var existingUser = await _userManager.FindByEmailAsync(loginVM.Email);
            if(existingUser == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Email or Password. Please try agian");
                return View(loginVM);
            }


            var existingUserClaims = await _userManager.GetClaimsAsync(existingUser);
            if (!existingUserClaims.Any(c => c.Type == CustomClaim.FullName))
                await _userManager.AddClaimAsync(existingUser, new Claim(CustomClaim.FullName, existingUser.FullName));

            var result = await _signInManager.PasswordSignInAsync(existingUser.UserName, loginVM.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)                                          
                return RedirectToAction("Index", "Home");                         
            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View(loginVM);
        }
        [HttpGet]
        [RedirectIfAuthenticated]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if(!ModelState.IsValid)          
                return View(registerVM);
            
            var newUser = new User()
            {
                Id = SnowflakeGenerator.Generate(),
                FullName = $"{registerVM.FirstName} {registerVM.LastName}",
                Email = registerVM.Email,
                UserName = registerVM.Email,
            };
            var existingUser = await _userManager.FindByEmailAsync(registerVM.Email);
            if(existingUser != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(registerVM);
            }
            var result = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, AppRoles.User);
                await _userManager.AddClaimAsync(newUser, new Claim(CustomClaim.FullName, newUser.FullName));
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(registerVM);
        }



        [HttpPost]
        public async Task<IActionResult> UpdatePassWord(UpdatePassWordVM updatePassWordVM)
        {
            if (!ModelState.IsValid)
            {
                // Lấy lỗi của NewPassword (nếu có)
                var newPasswordError = ModelState["NewPassword"]?.Errors.FirstOrDefault()?.ErrorMessage;
                if (!string.IsNullOrEmpty(newPasswordError))
                {
                    TempData["PasswordError"] = newPasswordError;
                }
                else
                {
                    TempData["PasswordError"] = "Invalid input";
                }
                TempData["ActiveTab"] = "Password";
                return RedirectToAction("Index", "Settings");
            }

            if (updatePassWordVM.NewPassword != updatePassWordVM.ConfirmPassword)
            {
                TempData["PasswordError"] = "Password and Confirm Password do not match";
                TempData["ActiveTab"] = "Password";
                return RedirectToAction("Index", "Settings");
            }

            var loggedInUser = await _userManager.GetUserAsync(User);
            var IsCurrentPasswordValid = await _userManager.CheckPasswordAsync(loggedInUser,
                updatePassWordVM.CurrentPassword);
            if (!IsCurrentPasswordValid)
            {
                TempData["PasswordError"] = "Current Password is incorrect";
                TempData["ActiveTab"] = "Password";
                return RedirectToAction("Index", "Settings");
            }
            if (updatePassWordVM.NewPassword == updatePassWordVM.CurrentPassword)
            {
                TempData["PasswordError"] = "New password must be different from the current password";
                TempData["ActiveTab"] = "Password";
                return RedirectToAction("Index", "Settings");
            }
            var result = await _userManager.ChangePasswordAsync(loggedInUser,
                updatePassWordVM.CurrentPassword, updatePassWordVM.NewPassword);
            if (result.Succeeded)
            {
                TempData["PasswordSuccess"] = "Password updated successfully";
                TempData["ActiveTab"] = "Password";
                await _signInManager.RefreshSignInAsync(loggedInUser);
               
            }

            return RedirectToAction("Index", "Settings");
        }

        [HttpPost]

        public async Task<IActionResult> UpdateProfile(ProfileVm profileVm)
        {
            var loggedInUser = await _userManager.GetUserAsync(User);
            if (!ModelState.IsValid)
            {
                TempData["ProfileError"] = "Input can only contain letters and cannot Contain 'admin'";
                TempData["ActiveTab"] = "Profile";
                return RedirectToAction("Index", "Settings");
            }
            if(loggedInUser == null)
                return RedirectToAction("Login");
            loggedInUser.FullName = profileVm.FullName;
            loggedInUser.UserName = profileVm.UserName;
            loggedInUser.Bio = profileVm.Bio;

            var result = await _userManager.UpdateAsync(loggedInUser);
            if(!result.Succeeded)
            {
                TempData["ProfileError"] = string.Join("; ", result.Errors.Select(e => e.Description));
                TempData["ActiveTab"] = "Profile";
              
            }
            TempData["ProfileErrorSuccessfull"] = "Profile updated successfully";
           await _signInManager.RefreshSignInAsync(loggedInUser);
            return RedirectToAction("Index", "Settings");
        }
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Authentication");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (info == null)
                return RedirectToAction("Login");

            // Lấy email từ nhiều provider (Google dùng ClaimTypes.Email, GitHub dùng "urn:github:email")
            var email = info.Principal.FindFirstValue(ClaimTypes.Email)
                ?? info.Principal.Claims.FirstOrDefault(c => c.Type == "urn:github:email")?.Value;
            // lưu avatar
            var avatarUrl = info.Principal.FindFirstValue("urn:google:picture")
                 ?? info.Principal.FindFirstValue("urn:github:avatar");

            if (string.IsNullOrEmpty(email))
            {
                TempData["LoginError"] = "Loggin failed! Are you setting email is private, Please set your email GitHub is public ";
                return RedirectToAction("Login");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var fullName = info.Principal.FindFirstValue(ClaimTypes.Name)
                    ?? info.Principal.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

                var newUser = new User()
                {
                    Id = SnowflakeGenerator.Generate(),
                    Email = email,
                    UserName = email,
                    FullName = fullName ?? email,
                    AvatarUrl = avatarUrl,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(newUser);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, AppRoles.User);
                    await _userManager.AddClaimAsync(newUser, new Claim(CustomClaim.FullName, newUser.FullName));
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }




        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
