using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class Reports
    {
        public Guid ReportId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public Guid? FormListId { get; set; }
        public string FormName { get; set; }
        public int? FieldItemId { get; set; }
        public string FieldName { get; set; }
        public string ReportType { get; set; }
        public string Color { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public bool? ShowInDashboard { get; set; }
        public bool? ShareWithAllUsers { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}