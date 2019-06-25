using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class LogTimes
    {
        public int LogTimeId { get; set; }
        public DateTime DateAdded { get; set; }
        public Guid LogGeneralId { get; set; }

        public virtual LogGenerals LogGeneral { get; set; }
    }
}
