using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class LogTimes
    {
        public int LogTimeId { get; set; }
        public DateTime DateAdded { get; set; }
        public Guid LogGeneralId { get; set; }

        public virtual LogGenerals LogGeneral { get; set; }
    }
}