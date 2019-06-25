using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class SavedFormSubmissions
    {
        public SavedFormSubmissions()
        {
            SavedSubmitValues = new HashSet<SavedSubmitValues>();
        }

        public Guid SavedSubmissionId { get; set; }
        public Guid FormId { get; set; }
        public string ReferrerUrl { get; set; }
        public string FormName { get; set; }
        public DateTime DateAdded { get; set; }
        public long TotalFileSize { get; set; }
        public Guid FormListId { get; set; }
        public int SubmissionId { get; set; }

        public virtual FormLists FormList { get; set; }
        public virtual ICollection<SavedSubmitValues> SavedSubmitValues { get; set; }
    }
}
