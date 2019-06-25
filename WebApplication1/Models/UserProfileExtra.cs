using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class UserProfileExtra
    {
        public int UserProfileExtraId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public DateTime CreatedAt { get; set; }
        public string StripeCustomerId { get; set; }
        public int UserId { get; set; }
    }
}
