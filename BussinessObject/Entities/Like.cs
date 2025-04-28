using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Entities
{
    public class Like
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public string PostId { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
        public virtual Post Post { get; set; } = null!;
    }
}
