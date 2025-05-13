using BussinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IAdminServices
    {
        Task<List<Post>> GetReportedPostsAsync();
    }
}
