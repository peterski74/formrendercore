using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class WebpagesUsersInRoles
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public virtual WebpagesRoles Role { get; set; }
        public virtual UserProfile User { get; set; }
    }
}
