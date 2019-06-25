using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class SavedSubmitValues
    {
        public int SubmitValuesId { get; set; }
        public int FieldId { get; set; }
        public string Value { get; set; }
        public string LabelName { get; set; }
        public int SubmissionId { get; set; }

        public virtual SavedFormSubmissions Submission { get; set; }
    }
}
