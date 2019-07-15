using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class SfCredentials
    {
        public Guid SfcredentialId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Consumerkey { get; set; }
        public string Consumersecret { get; set; }
        public string UserId { get; set; }
    }
}