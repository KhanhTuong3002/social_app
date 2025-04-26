using BusinessObject.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Entities
{
    public class Post : BaseEntity
    {
        public int PostId { get; set; }
        public string? ImageUrl { get; set; }
        public string? Content { get; set; }
        public int NrofRepost { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User user { get; set; }

    }
}
