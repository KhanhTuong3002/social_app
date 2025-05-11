using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Entities
{
    public class Notification
    {
        [Key]
        public string Id { get; set; } = SnowflakeGenerator.Generate();

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public User user { get; set; }

        public string Message { get; set; }
        public bool IsRead { get; set; }

        public string? PostId { get; set; } = null!;

        public string Type { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
