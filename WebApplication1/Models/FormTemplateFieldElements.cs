using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class FormTemplateFieldElements
    {
        public int FieldItemElementId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public bool IsDefault { get; set; }
        public int FieldItemId { get; set; }
        public string Value { get; set; }

        public virtual FormTemplateFields FieldItem { get; set; }
    }
}
