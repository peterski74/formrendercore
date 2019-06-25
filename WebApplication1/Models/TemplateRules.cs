using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class TemplateRules
    {
        public int TemplateRuleId { get; set; }
        public int ConditionField { get; set; }
        public string Condition { get; set; }
        public string ConditionMatchValue { get; set; }
        public string Action { get; set; }
        public string ActionField { get; set; }
        public Guid TemplateId { get; set; }
        public string Title { get; set; }
        public Guid FormListId { get; set; }

        public virtual FormTemplates Template { get; set; }
    }
}
