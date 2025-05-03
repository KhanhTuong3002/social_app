using BussinessObject.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetAllPostsAsync(string loggedInUser);

        Task<Post> GetPostByIdAsync(string postId);
        Task<List<Post>> GetAllFavoritePost(string loggedInUser);
        Task<Post> CreatePostAsync(Post post);
        Task<Post> RemovePostAsync(string postId);
        Task AddPostCommentAsync(Comment comment);
        Task RemovePostCommentAsync(string commentId);
        Task TogglePostLikeAsync(string postId, string userId);
        Task TogglePostFavoriteAsync(string postId, string userId);
        Task TogglePostVisibilityAsync(string postId, string userId);
        Task ReportPostAsync(string postId, string userId);
    }

   
}
