using BussinessObject.Entities;
using BussinessObject;
using DataAccess.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class HashtagServices : IHashtagServices
    {
        private readonly SociaDbContex _context;
        public HashtagServices(SociaDbContex context)
        {
            _context = context;
        }
        public async Task ProcessHashtagsForNewPostAsync(string postId, string content, string userId)
        {
            var posHashtags = HashtagHelper.GetHashtags(content);
            foreach (var hashtag in posHashtags)
            {
                var hashtagDb = await _context.HashTags
                    .Where(h => h.Name == hashtag)
                    .FirstOrDefaultAsync();
                if (hashtagDb != null)
                {
                    hashtagDb.Count += 1;
                    hashtagDb.UpdatedAt = DateTime.UtcNow;

                    _context.HashTags.Update(hashtagDb);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var newHashtag = new HashTag()
                    {
                        Id = SnowflakeGenerator.Generate(),
                        Name = hashtag,
                        Count = 1,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        PostId = postId,
                        UserId = userId,
                    };
                    await _context.HashTags.AddAsync(newHashtag);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task ProcessHashtagsForRemovePostAsync(string postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return;
            var hashtags = HashtagHelper.GetHashtags(content);
            foreach (var hashtag in hashtags)
            {
                var hashtagDb = await _context.HashTags
                    .Where(h => h.Name == hashtag)
                    .FirstOrDefaultAsync();
                if (hashtagDb != null)
                {
                    hashtagDb.Count -= 1;
                    hashtagDb.UpdatedAt = DateTime.UtcNow;

                    _context.HashTags.Update(hashtagDb);
                    await _context.SaveChangesAsync();
                }

            }
        }
    }
}
