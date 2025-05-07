using BussinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class UserServices : IUserServices
    {
        private readonly SociaDbContex _context;
        public UserServices(SociaDbContex context)
        {
            _context = context;
        }
        public async Task<User> GetUser(string loggedInUserId)
        {
            return await _context.Users.FirstOrDefaultAsync(n => n.Id == loggedInUserId) ?? new User();

        }

        public async Task<List<Post>> GetUserPosts(string userId)
        {
            var Allposts = await _context.Posts
                .Where(n => n.UserId == userId && n.Reports.Count < 5 && !n.IsDeleted)//restore a post to be public
                /*.Where(n => !n.isPrivate)*/
                .Include(n => n.user)
                .Include(n => n.Likes).ThenInclude(n => n.User)
                .Include(n => n.Comments).ThenInclude(n => n.User)
                .Include(n => n.Favorites).ThenInclude(n => n.User)
                .Include(n => n.Reports).ThenInclude(n => n.User)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Allposts;
        }

        public async Task UpdateUserProfile(string loggedInUserId, string avatarUrl)
        {
            var userDb = await _context.Users.FirstOrDefaultAsync(n => n.Id == loggedInUserId);
            if(userDb != null)
            {
                userDb.AvatarUrl = avatarUrl;
                _context.Users.Update(userDb);
                await _context.SaveChangesAsync();
            }
        }
    }
}
