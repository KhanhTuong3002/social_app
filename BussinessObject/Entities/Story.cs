using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Entities
{
    public class Story
    {
        public string Id { get; set; } = SnowflakeGenerator.Generate();
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public User User { get; set; }
    }
}
