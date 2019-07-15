using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class SfObjects
    {
        public SfObjects()
        {
            SfFields = new HashSet<SfFields>();
        }

        public Guid SfobjectId { get; set; }
        public string Name { get; set; }
        public string Common { get; set; }
        public string Label { get; set; }
        public bool? Custom { get; set; }
        public string UserId { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<SfFields> SfFields { get; set; }
    }
}