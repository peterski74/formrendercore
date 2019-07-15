using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class SubmissionNotes
    {
        public Guid SubmissionNoteId { get; set; }
        public string Note { get; set; }
        public string UserId { get; set; }
        public DateTime DateAdded { get; set; }
        public int SubmissionId { get; set; }

        public virtual FormSubmissions Submission { get; set; }
    }
}