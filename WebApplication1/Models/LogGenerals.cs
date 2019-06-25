using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class LogGenerals
    {
        public LogGenerals()
        {
            LogInteractWithForms = new HashSet<LogInteractWithForms>();
            LogTimes = new HashSet<LogTimes>();
        }

        public Guid LogGeneralId { get; set; }
        public string Browser { get; set; }
        public string Device { get; set; }
        public DateTime DateAdded { get; set; }
        public Guid FormListId { get; set; }
        public string Country { get; set; }
        public string BrowserVersion { get; set; }
        public DateTime? DateTimeSubmitted { get; set; }
        public string City { get; set; }
        public string Ipaddress { get; set; }

        public virtual FormLists FormList { get; set; }
        public virtual ICollection<LogInteractWithForms> LogInteractWithForms { get; set; }
        public virtual ICollection<LogTimes> LogTimes { get; set; }
    }
}
