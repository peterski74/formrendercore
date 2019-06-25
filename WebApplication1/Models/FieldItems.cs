using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class FieldItems
    {
        public FieldItems()
        {
            FieldItemElements = new HashSet<FieldItemElements>();
        }

        public int FieldItemId { get; set; }
        public string Title { get; set; }
        public string DefaultValue { get; set; }
        public bool Mandatory { get; set; }
        public string ElementType { get; set; }
        public string Properties { get; set; }
        public short MaxLenght { get; set; }
        public short FieldSize { get; set; }
        public string Tooltip { get; set; }
        public int ItemOrder { get; set; }
        public int MinCheck { get; set; }
        public int MaxCheck { get; set; }
        public string Mask { get; set; }
        public bool EmailValidate { get; set; }
        public string Content { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
        public Guid FormListId { get; set; }
        public string CrmMapping { get; set; }

        public virtual FormLists FormList { get; set; }
        public virtual ICollection<FieldItemElements> FieldItemElements { get; set; }
    }
}
