using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class FormTemplates
    {
        public FormTemplates()
        {
            FormTemplateFields = new HashSet<FormTemplateFields>();
            TemplateCalculations = new HashSet<TemplateCalculations>();
            TemplateRules = new HashSet<TemplateRules>();
        }

        public Guid TemplateId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Rating { get; set; }
        public int NoTimesUsed { get; set; }
        public bool Public { get; set; }
        public string Content { get; set; }
        public DateTime DateAdded { get; set; }
        public string Title { get; set; }
        public string Theme { get; set; }
        public string BackgroundImage { get; set; }
        public string LabelAlign { get; set; }
        public string LabelWidth { get; set; }
        public int FormWidth { get; set; }
        public bool Core { get; set; }
        public string IsTransparent { get; set; }
        public int? TransparentValue { get; set; }

        public virtual ICollection<FormTemplateFields> FormTemplateFields { get; set; }
        public virtual ICollection<TemplateCalculations> TemplateCalculations { get; set; }
        public virtual ICollection<TemplateRules> TemplateRules { get; set; }
    }
}