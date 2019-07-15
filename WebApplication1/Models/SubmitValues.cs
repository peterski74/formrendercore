using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class SubmitValues
    {
        public int SubmitValuesId { get; set; }
        public int FieldId { get; set; }
        public string Value { get; set; }
        public string LabelName { get; set; }
        public int SubmissionId { get; set; }

        public virtual FormSubmissions Submission { get; set; }
    }
}