using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class StripeAccounts
    {
        public StripeAccounts()
        {
            CreditCards = new HashSet<CreditCards>();
            SubscriptionPlans = new HashSet<SubscriptionPlans>();
            Subscriptions = new HashSet<Subscriptions>();
        }

        public int Id { get; set; }
        public bool LiveMode { get; set; }
        public string StripeLivePublicApiKey { get; set; }
        public string StripeLiveSecretApiKey { get; set; }
        public string StripeTestPublicApiKey { get; set; }
        public string StripeTestSecretApiKey { get; set; }

        public virtual ICollection<CreditCards> CreditCards { get; set; }
        public virtual ICollection<SubscriptionPlans> SubscriptionPlans { get; set; }
        public virtual ICollection<Subscriptions> Subscriptions { get; set; }
    }
}
