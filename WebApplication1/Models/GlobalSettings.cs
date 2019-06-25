using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class GlobalSettings
    {
        public int Id { get; set; }
        public string Dateformat { get; set; }
        public string Timezone { get; set; }
        public string Language { get; set; }
        public string InvoiceEmailTo { get; set; }
        public int ApplicationUserId { get; set; }
        public string ApplicationUserName { get; set; }
        public int ContactsUnreadCount { get; set; }
        public int SubmitsUnreadCount { get; set; }
        public bool ViewedDashboardTour { get; set; }
        public bool ViewedFormEditorTour { get; set; }
        public bool ViewedCrmtour { get; set; }
        public bool ViewedSubmitTour { get; set; }
        public bool ViewedInsightsTour { get; set; }
        public bool ViewedReportsTour { get; set; }
        public bool ViewedSettingsTour { get; set; }
    }
}
