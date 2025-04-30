using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Entities
{
    public class HashTag
    {
        [Key]
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Count { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey(nameof(Post))]
        public string PostId { get; set; } = null!;
        public Post Post { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public User User { get; set; }
    }
}
