using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class EmailTemplates
    {
        public int EmailTemplateId { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public Guid FormListId { get; set; }

        public virtual FormLists FormList { get; set; }
    }
}
