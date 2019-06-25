using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class LogNotificationEmails
    {
        public int LogNotificationEmailId { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string ApplicationUserName { get; set; }
    }
}
