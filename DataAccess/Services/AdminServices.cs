using BussinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class AdminServices : IAdminServices
    {
        private readonly SociaDbContex _sociaDbContex;
        public AdminServices(SociaDbContex sociaDbContex)
        {
            _sociaDbContex = sociaDbContex;
        }

        public async Task<List<Post>> GetReportedPostsAsync()
        {
            //var posts = await _context.Posts
            //    .Include(n => n.Reports)
            //    .Where(n => n.Reports.Count > 5 && !n.IsDeleted)
            //    .ToListAsync();

            var posts = await _sociaDbContex.Posts
                .Include(n => n.user)
                .Where(n => n.NrofRepost > 5 && !n.IsDeleted)
                .ToListAsync();

            return posts;
        }
    }
}
