using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class UserProfile
    {
        public UserProfile()
        {
            WebpagesUsersInRoles = new HashSet<WebpagesUsersInRoles>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }

        public virtual ICollection<WebpagesUsersInRoles> WebpagesUsersInRoles { get; set; }
    }
}