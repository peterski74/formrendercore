using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Calculations
    {
        public int CalculationId { get; set; }
        public string Title { get; set; }
        public string CalcContent { get; set; }
        public int ResultField { get; set; }
        public Guid FormListId { get; set; }
        public string Formula { get; set; }
        public string ResultFieldOut { get; set; }

        public virtual FormLists FormList { get; set; }
    }
}
