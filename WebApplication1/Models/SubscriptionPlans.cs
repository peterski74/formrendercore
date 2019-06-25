using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class SubscriptionPlans
    {
        public SubscriptionPlans()
        {
            Subscriptions = new HashSet<Subscriptions>();
        }

        public int Id { get; set; }
        public string FriendlyId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public int TrialPeriodInDays { get; set; }
        public bool Disabled { get; set; }
        public int? StripeAccountId { get; set; }
        public int UsersAllowed { get; set; }

        public virtual StripeAccounts StripeAccount { get; set; }
        public virtual ICollection<Subscriptions> Subscriptions { get; set; }
    }
}
