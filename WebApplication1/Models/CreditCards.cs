using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class CreditCards
    {
        public string StripeId { get; set; }
        public string StripeToken { get; set; }
        public string Name { get; set; }
        public string Last4 { get; set; }
        public string Type { get; set; }
        public string Fingerprint { get; set; }
        public string AddressCity { get; set; }
        public string AddressCountry { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressState { get; set; }
        public string AddressZip { get; set; }
        public string Cvc { get; set; }
        public string ExpirationMonth { get; set; }
        public string ExpirationYear { get; set; }
        public string ApplicationUserId { get; set; }
        public int? StripeAccountId { get; set; }
        public int CreditCardId { get; set; }
        public int InvoiceId { get; set; }
        public string Organisation { get; set; }
        public string StripeCustomerId { get; set; }
        public string FundingType { get; set; }

        public virtual StripeAccounts StripeAccount { get; set; }
    }
}