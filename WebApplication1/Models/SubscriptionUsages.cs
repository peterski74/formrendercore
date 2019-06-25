using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class SubscriptionUsages
    {
        public int Id { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public int Submissions { get; set; }
        public long Uploads { get; set; }
        public int ApplicationUserId { get; set; }
        public string ApplicationUserName { get; set; }
        public int AllowedSubmissions { get; set; }
        public int AllowedUploads { get; set; }
        public int AllowedUploadsPerForm { get; set; }
        public int UsersTotal { get; set; }
        public int AllowedUsers { get; set; }
    }
}
