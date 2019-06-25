using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class LogInteractWithForms
    {
        public int LogInteractWithFormId { get; set; }
        public Guid LogGeneralId { get; set; }

        public virtual LogGenerals LogGeneral { get; set; }
    }
}
