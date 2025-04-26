using BusinessObject.Entites;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.Entities
{
    public class User : IdentityUser
    {
        public string Id { get; set; }
        public string? FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
