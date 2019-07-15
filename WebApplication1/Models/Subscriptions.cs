using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class Subscriptions
    {
        public int Id { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public DateTime? TrialStart { get; set; }
        public DateTime? TrialEnd { get; set; }
        public int SubscriptionPlanId { get; set; }
        public int ApplicationUserId { get; set; }
        public string StripeId { get; set; }
        public int? StripeAccountId { get; set; }
        public string ApplicationUserName { get; set; }
        public DateTime? PaidStart { get; set; }
        public DateTime? PaidEnd { get; set; }
        public string StripeSubscriptionId { get; set; }
        public bool Paid { get; set; }

        public virtual StripeAccounts StripeAccount { get; set; }
        public virtual SubscriptionPlans SubscriptionPlan { get; set; }
    }
}