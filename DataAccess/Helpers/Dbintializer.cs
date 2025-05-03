using BussinessObject;
using BussinessObject.Entities;
using DataAccess.Helpers.Constants;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Helpers
{
    public static class DbInitializer
    {
        public static async Task SeedUserAndRoleAsync(
            UserManager<User> userManager,
            RoleManager<IdentityRole<string>> roleManager)
        {
            // Seed roles
            if (!roleManager.Roles.Any())
            {
                foreach (var roleName in AppRoles.All)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        var role = new IdentityRole<string>
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = roleName,
                            NormalizedName = roleName.ToUpper()
                        };

                        var result = await roleManager.CreateAsync(role);
                        if (!result.Succeeded)
                        {
                            throw new Exception($"Failed to create role: {roleName}");
                        }
                    }
                }
            }

            // Seed users with roles
            if (!userManager.Users.Any(u => !string.IsNullOrEmpty(u.Email)))
            {
                var userPassword = "Admin@123";

                var newUser = new User
                {
                    Id = SnowflakeGenerator.Generate(),
                    UserName = "User001",
                    Email = "Votuongpro2017@gmail.com",
                    FullName = "Khanh Tuong",
                    AvatarUrl = "/images/logo.png",
                    EmailConfirmed = true,
                };

                var resultUser = await userManager.CreateAsync(newUser, userPassword);
                if (resultUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, AppRoles.User);
                }
                else
                {
                    throw new Exception("Failed to create regular user.");
                }

                var newAdmin = new User
                {
                    Id = SnowflakeGenerator.Generate(),
                    UserName = "Admin001",
                    Email = "AdminVotuongpro2017@gmail.com",
                    FullName = "Khanh Tuong Admin",
                    AvatarUrl = "/images/logo2.png",
                    EmailConfirmed = true,
                };

                var resultAdmin = await userManager.CreateAsync(newAdmin, userPassword);
                if (resultAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, AppRoles.Admin);
                }
                else
                {
                    throw new Exception("Failed to create admin user.");
                }
            }
        }

        public static async Task SeedAsync(SociaDbContex context)
        {
            /*
            if (!context.Users.Any() && !context.Posts.Any())
            {
                var newUser = new User
                {
                    Id = SnowflakeGenerator.Generate(),
                    FullName = "Khanh Tuong",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    AvatarUrl = "/images/logo2.png"
                };

                await context.Users.AddAsync(newUser);
                await context.SaveChangesAsync(); // Save to generate UserId

                var newPostWithImage = new Post
                {
                    ImageUrl = "https://cdn.tgdd.vn/Files/2015/01/06/596407/he-dieu-hanh-windows-la-gi--2.jpg",
                    Content = "Hello World! this is the first post in this social media",
                    NrofRepost = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = newUser.Id,
                };

                var newPostWithoutImage = new Post
                {
                    ImageUrl = "",
                    Content = "Hello World!",
                    NrofRepost = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = newUser.Id,
                };

                await context.Posts.AddRangeAsync(newPostWithImage, newPostWithoutImage);
                await context.SaveChangesAsync();
            }
            */
        }
    }
}