﻿using BussinessObject.Entities;
using DataAccess.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class PostService : IPostService
    {
        private readonly SociaDbContex _context;
        private readonly INotificationService _notificationService;
        public PostService(SociaDbContex context , INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<List<Post>> GetAllPostsAsync(string loggedInUser)
        {
            var Allposts = await _context.Posts
                 .Where(n => (!n.isPrivate || n.UserId == loggedInUser.ToString()) && n.Reports.Count < 5 && !n.IsDeleted)//restore a post to be public
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

        public async Task<Post> GetPostByIdAsync(string postId)
        {
            var postDb = await _context.Posts
              .Include(n => n.user)
              .Include(n => n.Likes).ThenInclude(n => n.User)
              .Include(n => n.Comments).ThenInclude(n => n.User)
              .Include(n => n.Favorites).ThenInclude(n => n.User)
              .FirstOrDefaultAsync(n => n.PostId == postId);
            return postDb;
        }

        public async Task<List<Post>> GetAllFavoritePost(string loggedInUser)
        {
            var allFavoritePosts = await _context.Favorites
                .Where(n => n.UserId == loggedInUser &&
                            !n.Post.IsDeleted &&
                            n.Post.Reports.Count < 5)
                .Include(n => n.Post).ThenInclude(p => p.user)
                .Include(n => n.Post).ThenInclude(p => p.Reports)
                .Include(n => n.Post).ThenInclude(p => p.Comments).ThenInclude(c => c.User)
                .Include(n => n.Post).ThenInclude(p => p.Likes).ThenInclude(l => l.User)
                .Select(n => n.Post)
                .ToListAsync();

            return allFavoritePosts;
        }

        public async Task AddPostCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<Post> CreatePostAsync(Post post)
        {

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            return post;
        }


        public async Task<Post> RemovePostAsync(string postId)
        {
            var postDb = await _context.Posts.FirstOrDefaultAsync(n => n.PostId == postId);
            if (postDb != null)
            {
                postDb.IsDeleted = true;
                _context.Posts.Update(postDb);
                await _context.SaveChangesAsync();
            }
            return postDb;
        }

        public async Task RemovePostCommentAsync(string commentId)
        {
            var commentDb = await _context.Comments.FirstOrDefaultAsync(n => n.CommentId == commentId);
            if (commentDb != null)
            {
                _context.Comments.Remove(commentDb);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReportPostAsync(string postId, string userId)
        {
            var newReport = new Report()
            {
                CreatedAt = DateTime.UtcNow,
                PostId = postId,
                UserId = userId
            };

            await _context.Reports.AddAsync(newReport);
            await _context.SaveChangesAsync();

        }

        public async Task<GetNotificationDto> TogglePostFavoriteAsync(string postId, string userId)
        {
            var response = new GetNotificationDto()
            {
                Success = true,
                SendNotification = false,
            };

            //check if user favorite the post
            var favorite = await _context.Favorites.
                Where(l => l.PostId == postId && l.UserId == userId).FirstOrDefaultAsync();

            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newFavorite = new Favorite()
                {
                    PostId = postId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                };
                await _context.Favorites.AddAsync(newFavorite);
                await _context.SaveChangesAsync();

                response.SendNotification = true;
            }
            return response;
        }

        public async Task<GetNotificationDto> TogglePostLikeAsync(string postId, string userId)
        {
            var response = new GetNotificationDto()
            {
                Success = true,
                SendNotification = false,
            };
            //check if user liked the post
            var like = await _context.Likes.
                Where(l => l.PostId == postId && l.UserId == userId).FirstOrDefaultAsync();

            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newLike = new Like()
                {
                    PostId = postId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                };
                await _context.Likes.AddAsync(newLike);
                await _context.SaveChangesAsync();

                response.SendNotification = true;
            }
            return response;
        }

        public async Task TogglePostVisibilityAsync(string postId, string userId)
        {
            var post = await _context.Posts
               .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (post != null)
            {
                post.isPrivate = !post.isPrivate;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
            }
        }

       
    }
}
