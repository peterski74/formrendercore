using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class Invoices
    {
        public string StripeId { get; set; }
        public string StripeCustomerId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public int? Subtotal { get; set; }
        public int? Total { get; set; }
        public bool? Attempted { get; set; }
        public bool? Closed { get; set; }
        public bool? Paid { get; set; }
        public int? AttemptCount { get; set; }
        public int? AmountDue { get; set; }
        public int? StartingBalance { get; set; }
        public int? EndingBalance { get; set; }
        public DateTime? NextPaymentAttempt { get; set; }
        public int? Charge { get; set; }
        public int? Discount { get; set; }
        public int? ApplicationFee { get; set; }
        public string Currency { get; set; }
        public string UserId { get; set; }
        public int InvoiceId { get; set; }
        public int? Tax { get; set; }
        public string ChargeStripeId { get; set; }
        public string ChargeStatus { get; set; }
    }
}