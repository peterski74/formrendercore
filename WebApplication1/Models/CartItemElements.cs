using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class CartItemElements
    {
        public int CartItemElementId { get; set; }
        public string ItemName { get; set; }
        public string Amount { get; set; }
        public int Order { get; set; }
        public bool IsDefault { get; set; }
        public int PayPalId { get; set; }

        public virtual PayPalAccounts PayPal { get; set; }
    }
}
