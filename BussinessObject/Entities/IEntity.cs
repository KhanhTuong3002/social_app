using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Entites
{
    public interface IEntity<TId>
    {
        [Key] public TId Id { get; set; }
    }

    public interface IEntity : IEntity<string>
    {
    }
}
