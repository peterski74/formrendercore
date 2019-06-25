using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class FormLists
    {
        public FormLists()
        {
            Calculations = new HashSet<Calculations>();
            EmailTemplates = new HashSet<EmailTemplates>();
            FieldItems = new HashSet<FieldItems>();
            FormSubmissions = new HashSet<FormSubmissions>();
            LogGenerals = new HashSet<LogGenerals>();
            PayPalAccounts = new HashSet<PayPalAccounts>();
            Rules = new HashSet<Rules>();
            SavedFormSubmissions = new HashSet<SavedFormSubmissions>();
        }

        public Guid FormListId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Theme { get; set; }
        public string BackgroundImage { get; set; }
        public string LabelAlign { get; set; }
        public string LabelWidth { get; set; }
        public int FormWidth { get; set; }
        public DateTime DateAdded { get; set; }
        public string NotificationEmail { get; set; }
        public int? NotificationEmailTemplate { get; set; }
        public int? NotificationEmailFieldSubmitter { get; set; }
        public int? NotificationEmailFieldSubmitterTemplate { get; set; }
        public string Status { get; set; }
        public string ConfirmationMessage { get; set; }
        public string OnSubmit { get; set; }
        public string RedirectUrl { get; set; }
        public string Save { get; set; }
        public string Captcha { get; set; }
        public string ValidateLanguage { get; set; }
        public string IsTransparent { get; set; }
        public int? TransparentValue { get; set; }

        public virtual ICollection<Calculations> Calculations { get; set; }
        public virtual ICollection<EmailTemplates> EmailTemplates { get; set; }
        public virtual ICollection<FieldItems> FieldItems { get; set; }
        public virtual ICollection<FormSubmissions> FormSubmissions { get; set; }
        public virtual ICollection<LogGenerals> LogGenerals { get; set; }
        public virtual ICollection<PayPalAccounts> PayPalAccounts { get; set; }
        public virtual ICollection<Rules> Rules { get; set; }
        public virtual ICollection<SavedFormSubmissions> SavedFormSubmissions { get; set; }
    }
}
