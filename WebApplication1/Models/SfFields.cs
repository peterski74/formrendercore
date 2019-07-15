using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class SfFields
    {
        public SfFields()
        {
            SfElements = new HashSet<SfElements>();
        }

        public Guid SffieldId { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public string Lenght { get; set; }
        public string DefaultValue { get; set; }
        public bool? Nillable { get; set; }
        public bool? Updateable { get; set; }
        public string ReferenceTo { get; set; }
        public Guid? SfobjectId { get; set; }
        public string SfobjectName { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual SfObjects Sfobject { get; set; }
        public virtual ICollection<SfElements> SfElements { get; set; }
    }
}