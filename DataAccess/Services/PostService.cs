using BussinessObject.Entities;
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
        public PostService(SociaDbContex context)
        {
            _context = context;
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

        public async Task AddPostCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<Post> CreatePostAsync(Post post, IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (image.ContentType.Contains("image"))
                {
                    string rootFolderPathImage = Path.Combine(rootFolderPath, "images/posts");
                    Directory.CreateDirectory(rootFolderPathImage); // Create the directory if it doesn't exist

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string filePath = Path.Combine(rootFolderPathImage, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    //set the image url to the new post
                    post.ImageUrl = "/images/posts/" + fileName; // Set the image URL to the new post
                }
            }
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

        public async Task TogglePostFavoriteAsync(string postId, string userId)
        {

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
            }
        }

        public async Task TogglePostLikeAsync(string postId, string userId)
        {
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
            }
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
