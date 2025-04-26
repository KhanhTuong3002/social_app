using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Entities
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        public string? ImageUrl { get; set; }
        public string? Content { get; set; }
        public int NrofRepost { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
