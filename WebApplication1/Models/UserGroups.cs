using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class UserGroups
    {
        public int UserGroupId { get; set; }
        public Guid GroupId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool SubscriptionOwner { get; set; }
        public bool Deleted { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
