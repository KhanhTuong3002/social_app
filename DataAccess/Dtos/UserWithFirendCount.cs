using BussinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dtos
{
    public class UserWithFirendCount
    {
        public User user { get; set; }
        public int FriendCount { get; set; }
    }
}
