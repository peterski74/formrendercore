using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class SubmitProductOrders
    {
        public int SubmitValuesId { get; set; }
        public int PayPalId { get; set; }
        public int PayPalFieldId { get; set; }
        public string PaidDate { get; set; }
        public string OrderNo { get; set; }
        public int SubmissionId { get; set; }

        public virtual FormSubmissions Submission { get; set; }
    }
}