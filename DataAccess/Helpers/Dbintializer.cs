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
                    Id = SnowflakeGenerator.Generate(),
                    FullName = "Khanh Tuong",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    AvatarUrl = "/images/logo2.png"
                };

                await context.Users.AddAsync(newUser);
                await context.SaveChangesAsync(); // Save to generate UserId

                var newPostWithImage = new Post
                {
                    ImageUrl = "/images/placeholders/miao.png", // <-- suggested file extension
                    Content = "Hello World!",
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
        }
    }
}
