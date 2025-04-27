using BussinessObject;
using BussinessObject.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Helpers
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(SociaDbContex context)
        {
            if (!context.Users.Any() && !context.Posts.Any())
            {
                var newUser = new User
                {
                    UserId = SnowflakeGenerator.Generate(),
                    FullName = "Khanh Tuong",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    AvatarUrl = "/images/logo2.png"
                };

                await context.Users.AddAsync(newUser);
                await context.SaveChangesAsync(); // Save to generate UserId

                var newPostWithImage = new Post
                {
                    ImageUrl = "https://cdn.tgdd.vn/Files/2015/01/06/596407/he-dieu-hanh-windows-la-gi--2.jpg", // <-- suggested file extension
                    Content = "Hello World! this is the first post in this social media",
                    NrofRepost = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = newUser.UserId,
                };

                var newPostWithoutImage = new Post
                {
                    ImageUrl = "",
                    Content = "Hello World!",
                    NrofRepost = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = newUser.UserId,
                };

                await context.Posts.AddRangeAsync(newPostWithImage, newPostWithoutImage);
                await context.SaveChangesAsync();
            }
        }
    }
}
