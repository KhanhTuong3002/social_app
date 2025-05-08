using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Entities
{
    public class FriendShip
    {
        [Key]
        public string Id { get; set; } = SnowflakeGenerator.Generate();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string SenderId { get; set; }
        public virtual User Sender { get; set; }
        public string ReceiverId { get; set; }
        public virtual User Receiver { get; set; }
       
    }
}
