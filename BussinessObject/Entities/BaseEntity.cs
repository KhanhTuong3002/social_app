using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BussinessObject.Entites;
using BussinessObject;

namespace BusinessObject.Entites
{
    [PrimaryKey(nameof(Id))]
    public abstract class BaseEntity<TId> : IEntity<TId>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset? LastUpdated { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public virtual TId Id { get; set; } = default!;
    }

    [PrimaryKey(nameof(Id))]
    public abstract class BaseEntity : BaseEntity<string>, IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(20)]
        [Key]
        public override string Id { get; set; } = SnowflakeGenerator.Generate();
    }
}
