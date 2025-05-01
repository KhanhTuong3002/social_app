using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IHashtagServices
    {
        Task ProcessHashtagsForNewPostAsync(string postId, string content,string userId);
        Task ProcessHashtagsForRemovePostAsync(string postId, string content);
    }
}
