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
