using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class PayPalAccounts
    {
        public PayPalAccounts()
        {
            CartItemElements = new HashSet<CartItemElements>();
        }

        public int PayPalId { get; set; }
        public string Business { get; set; }
        public string CurrencyCode { get; set; }
        public string Return { get; set; }
        public string NotifyUrl { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime DateAdded { get; set; }
        public string Status { get; set; }
        public bool Mandatory { get; set; }
        public string Title { get; set; }
        public string Tooltip { get; set; }
        public Guid FormListId { get; set; }

        public virtual FormLists FormList { get; set; }
        public virtual ICollection<CartItemElements> CartItemElements { get; set; }
    }
}