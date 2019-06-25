using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class LogEmails
    {
        public int LogEmailId { get; set; }
        public DateTime SendDate { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string ApplicationUserName { get; set; }
        public string Cid { get; set; }
    }
}
