using BusinessObject.Entites;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Entities
{
    public class User
    {
        [Key]
        public string UserId { get; set; } = SnowflakeGenerator.Generate();
        public string? FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
        public ICollection<Story> Stories { get; set; } = new List<Story>();
        public ICollection<HashTag> HashTags { get; set; } = new List<HashTag>();
    }
}
