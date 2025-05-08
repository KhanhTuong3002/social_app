using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Entities
{
    public class FriendRequest
    {
        [Key]
        public string Id { get; set; }= SnowflakeGenerator.Generate(); 
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(User))]
        public string SenderId { get; set; }
        public User sender { get; set; }
      
        [ForeignKey(nameof(User))]
        public string ReceiverId { get; set; }
        public User receiver { get; set; }

    }
}
