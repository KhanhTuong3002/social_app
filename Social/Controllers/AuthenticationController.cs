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

            var result = await _signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password, isPersistent: false, lockoutOnFailure: false);

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
            if(updatePassWordVM.NewPassword != updatePassWordVM.ConfirmPassword)
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

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
