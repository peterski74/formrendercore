using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class SfElements
    {
        public Guid SfelementId { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string Defaultvalue { get; set; }
        public Guid? SffieldId { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual SfFields Sffield { get; set; }
    }
}