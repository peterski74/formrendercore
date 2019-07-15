using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class LogLastLogins
    {
        public int LogLastLoginId { get; set; }
        public int ApplicationUserId { get; set; }
        public string ApplicationUserName { get; set; }
        public DateTime LoginDate { get; set; }
        public string RefererLocation { get; set; }
    }
}