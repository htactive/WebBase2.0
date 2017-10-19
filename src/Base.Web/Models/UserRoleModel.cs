using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Base.Web.Models
{
    public class UserRoleModel
    {
        public int Id { get; set; }
        public int? RoleId { get; set; }
        public int? UserId { get; set; }
        
        public UserModel User { get; set; }
        
        public RoleModel Role { get; set; }
    }
}
