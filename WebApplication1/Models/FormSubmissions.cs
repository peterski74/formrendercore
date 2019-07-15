using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class FormSubmissions
    {
        public FormSubmissions()
        {
            SubmissionNotes = new HashSet<SubmissionNotes>();
            SubmitProductOrders = new HashSet<SubmitProductOrders>();
            SubmitValues = new HashSet<SubmitValues>();
        }

        public int SubmissionId { get; set; }
        public Guid FormId { get; set; }
        public string ReferrerUrl { get; set; }
        public string FormName { get; set; }
        public DateTime DateAdded { get; set; }
        public long TotalFileSize { get; set; }
        public Guid? FormListId { get; set; }
        public string Priority { get; set; }
        public bool Starred { get; set; }
        public bool Viewed { get; set; }
        public int ClientFriendlyId { get; set; }
        public string SearchField { get; set; }
        public string RelatedToContactId { get; set; }

        public virtual FormLists FormList { get; set; }
        public virtual ICollection<SubmissionNotes> SubmissionNotes { get; set; }
        public virtual ICollection<SubmitProductOrders> SubmitProductOrders { get; set; }
        public virtual ICollection<SubmitValues> SubmitValues { get; set; }
    }
}