using BussinessObject.Entities;
using DataAccess.Dtos;
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
        Task<GetNotificationDto> TogglePostLikeAsync(string postId, string userId);
        Task<GetNotificationDto> TogglePostFavoriteAsync(string postId, string userId);
        Task TogglePostVisibilityAsync(string postId, string userId);
        Task ReportPostAsync(string postId, string userId);
    }

   
}
