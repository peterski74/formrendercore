using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Rules
    {
        public int RuleId { get; set; }
        public int ConditionField { get; set; }
        public string Condition { get; set; }
        public string ConditionMatchValue { get; set; }
        public string Action { get; set; }
        public string ActionField { get; set; }
        public Guid FormListId { get; set; }
        public string Title { get; set; }

        public virtual FormLists FormList { get; set; }
    }
}
