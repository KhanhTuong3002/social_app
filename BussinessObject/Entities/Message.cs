using BusinessObject.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Entities
{
    public class Message : BaseEntity
    {
        [ForeignKey(nameof(Sender))]
        public string? SenderId { get; set; }

        [ForeignKey(nameof(Receiver))]
        public string? ReceiverId { get; set; }

        public string? Content { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        public virtual User Sender { get; set; } = null!;
        public virtual User Receiver { get; set; } = null!;
    }
}
