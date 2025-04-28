﻿using BusinessObject.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Entities
{
    public class Post
    {
        [Key]
        public string PostId { get; set; } = SnowflakeGenerator.Generate();
        public string? ImageUrl { get; set; }
        public string? Content { get; set; }
        public bool isPrivate { get; set; } = false;
        public int NrofRepost { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public User user { get; set; }

        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    }
}
