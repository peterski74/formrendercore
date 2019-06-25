using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApplication1.Models
{
    public partial class frmy01_DevContext : DbContext
    {
        public frmy01_DevContext()
        {
        }

        public frmy01_DevContext(DbContextOptions<frmy01_DevContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BackgroundImages> BackgroundImages { get; set; }
        public virtual DbSet<Calculations> Calculations { get; set; }
        public virtual DbSet<CartItemElements> CartItemElements { get; set; }
        public virtual DbSet<ContactNotes> ContactNotes { get; set; }
        public virtual DbSet<Contacts> Contacts { get; set; }
        public virtual DbSet<CreditCards> CreditCards { get; set; }
        public virtual DbSet<EmailTemplates> EmailTemplates { get; set; }
        public virtual DbSet<FieldItemElements> FieldItemElements { get; set; }
        public virtual DbSet<FieldItems> FieldItems { get; set; }
        public virtual DbSet<FormLists> FormLists { get; set; }
        public virtual DbSet<FormSubmissions> FormSubmissions { get; set; }
        public virtual DbSet<FormTemplateFieldElements> FormTemplateFieldElements { get; set; }
        public virtual DbSet<FormTemplateFields> FormTemplateFields { get; set; }
        public virtual DbSet<FormTemplates> FormTemplates { get; set; }
        public virtual DbSet<GlobalSettings> GlobalSettings { get; set; }
        public virtual DbSet<Invoices> Invoices { get; set; }
        public virtual DbSet<LogEmails> LogEmails { get; set; }
        public virtual DbSet<LogGenerals> LogGenerals { get; set; }
        public virtual DbSet<LogInteractWithForms> LogInteractWithForms { get; set; }
        public virtual DbSet<LogLastLogins> LogLastLogins { get; set; }
        public virtual DbSet<LogNotificationEmails> LogNotificationEmails { get; set; }
        public virtual DbSet<LogTimes> LogTimes { get; set; }
        public virtual DbSet<MembershipTypes> MembershipTypes { get; set; }
        public virtual DbSet<PayPalAccounts> PayPalAccounts { get; set; }
        public virtual DbSet<Reports> Reports { get; set; }
        public virtual DbSet<Rules> Rules { get; set; }
        public virtual DbSet<SavedFormSubmissions> SavedFormSubmissions { get; set; }
        public virtual DbSet<SavedSubmitValues> SavedSubmitValues { get; set; }
        public virtual DbSet<StripeAccounts> StripeAccounts { get; set; }
        public virtual DbSet<SubmissionNotes> SubmissionNotes { get; set; }
        public virtual DbSet<SubmitProductOrders> SubmitProductOrders { get; set; }
        public virtual DbSet<SubmitValues> SubmitValues { get; set; }
        public virtual DbSet<SubscriptionPlans> SubscriptionPlans { get; set; }
        public virtual DbSet<SubscriptionUsages> SubscriptionUsages { get; set; }
        public virtual DbSet<Subscriptions> Subscriptions { get; set; }
        public virtual DbSet<TemplateCalculations> TemplateCalculations { get; set; }
        public virtual DbSet<TemplateRules> TemplateRules { get; set; }
        public virtual DbSet<UserGroups> UserGroups { get; set; }
        public virtual DbSet<UserProfile> UserProfile { get; set; }
        public virtual DbSet<UserProfileExtra> UserProfileExtra { get; set; }
        public virtual DbSet<WebpagesMembership> WebpagesMembership { get; set; }
        public virtual DbSet<WebpagesOauthMembership> WebpagesOauthMembership { get; set; }
        public virtual DbSet<WebpagesRoles> WebpagesRoles { get; set; }
        public virtual DbSet<WebpagesUsersInRoles> WebpagesUsersInRoles { get; set; }

        // Unable to generate entity type for table 'dbo.ReportPermissions'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=frmy01_Dev2;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<BackgroundImages>(entity =>
            {
                entity.Property(e => e.Category).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(50);
            });

            modelBuilder.Entity<Calculations>(entity =>
            {
                entity.HasKey(e => e.CalculationId)
                    .HasName("PK_dbo.Calculations");

                entity.HasOne(d => d.FormList)
                    .WithMany(p => p.Calculations)
                    .HasForeignKey(d => d.FormListId)
                    .HasConstraintName("FK_dbo.Calculations_dbo.FormLists_FormListId");
            });

            modelBuilder.Entity<CartItemElements>(entity =>
            {
                entity.HasKey(e => e.CartItemElementId)
                    .HasName("PK_dbo.CartItemElements");

                entity.Property(e => e.ItemName).HasColumnName("Item_Name");

                entity.HasOne(d => d.PayPal)
                    .WithMany(p => p.CartItemElements)
                    .HasForeignKey(d => d.PayPalId)
                    .HasConstraintName("FK_dbo.CartItemElements_dbo.PayPalAccounts_PayPalId");
            });

            modelBuilder.Entity<ContactNotes>(entity =>
            {
                entity.HasKey(e => e.ContactNoteId)
                    .HasName("PK_dbo.ContactNotes");

                entity.Property(e => e.ContactNoteId).ValueGeneratedNever();

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.Contact)
                    .WithMany(p => p.ContactNotes)
                    .HasForeignKey(d => d.ContactId)
                    .HasConstraintName("FK_dbo.ContactNotes_dbo.Contacts_ContactId");
            });

            modelBuilder.Entity<Contacts>(entity =>
            {
                entity.HasKey(e => e.ContactId)
                    .HasName("PK_dbo.Contacts");

                entity.Property(e => e.ContactId).ValueGeneratedNever();

                entity.Property(e => e.CrmAccountName).HasColumnName("CRM_AccountName");

                entity.Property(e => e.CrmAccountType).HasColumnName("CRM_AccountType");

                entity.Property(e => e.CrmAssignedTo).HasColumnName("CRM_AssignedTo");

                entity.Property(e => e.CrmCountry).HasColumnName("CRM_Country");

                entity.Property(e => e.CrmDateAdded)
                    .HasColumnName("CRM_DateAdded")
                    .HasColumnType("datetime");

                entity.Property(e => e.CrmDescription).HasColumnName("CRM_Description");

                entity.Property(e => e.CrmDoNotCall).HasColumnName("CRM_DoNotCall");

                entity.Property(e => e.CrmEmail).HasColumnName("CRM_Email");

                entity.Property(e => e.CrmFirstName).HasColumnName("CRM_FirstName");

                entity.Property(e => e.CrmLastName).HasColumnName("CRM_LastName");

                entity.Property(e => e.CrmLeadSource).HasColumnName("CRM_LeadSource");

                entity.Property(e => e.CrmMobile).HasColumnName("CRM_Mobile");

                entity.Property(e => e.CrmOpportunity).HasColumnName("CRM_Opportunity");

                entity.Property(e => e.CrmPhone).HasColumnName("CRM_Phone");

                entity.Property(e => e.CrmPostcode).HasColumnName("CRM_Postcode");

                entity.Property(e => e.CrmState).HasColumnName("CRM_State");

                entity.Property(e => e.CrmStreet).HasColumnName("CRM_Street");

                entity.Property(e => e.CrmSuburb).HasColumnName("CRM_Suburb");

                entity.Property(e => e.CrmTitle).HasColumnName("CRM_Title");

                entity.Property(e => e.CrmWebsite).HasColumnName("CRM_Website");

                entity.Property(e => e.RelatedToSubmissionId).HasMaxLength(50);

                entity.Property(e => e.UserId).IsRequired();
            });

            modelBuilder.Entity<CreditCards>(entity =>
            {
                entity.HasKey(e => e.CreditCardId)
                    .HasName("PK_dbo.CreditCards");

                entity.Property(e => e.AddressCity).IsRequired();

                entity.Property(e => e.AddressCountry).IsRequired();

                entity.Property(e => e.AddressLine1).IsRequired();

                entity.Property(e => e.AddressZip).IsRequired();

                entity.Property(e => e.Cvc)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.Property(e => e.ExpirationMonth).IsRequired();

                entity.Property(e => e.ExpirationYear).IsRequired();

                entity.Property(e => e.FundingType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.StripeAccountId).HasColumnName("StripeAccount_Id");

                entity.HasOne(d => d.StripeAccount)
                    .WithMany(p => p.CreditCards)
                    .HasForeignKey(d => d.StripeAccountId)
                    .HasConstraintName("FK_dbo.CreditCards_dbo.StripeAccounts_StripeAccount_Id");
            });

            modelBuilder.Entity<EmailTemplates>(entity =>
            {
                entity.HasKey(e => e.EmailTemplateId)
                    .HasName("PK_dbo.EmailTemplates");

                entity.HasOne(d => d.FormList)
                    .WithMany(p => p.EmailTemplates)
                    .HasForeignKey(d => d.FormListId)
                    .HasConstraintName("FK_dbo.EmailTemplates_dbo.FormLists_FormListId");
            });

            modelBuilder.Entity<FieldItemElements>(entity =>
            {
                entity.HasKey(e => e.FieldItemElementId)
                    .HasName("PK_dbo.FieldItemElements");

                entity.HasOne(d => d.FieldItem)
                    .WithMany(p => p.FieldItemElements)
                    .HasForeignKey(d => d.FieldItemId)
                    .HasConstraintName("FK_dbo.FieldItemElements_dbo.FieldItems_FieldItemId");
            });

            modelBuilder.Entity<FieldItems>(entity =>
            {
                entity.HasKey(e => e.FieldItemId)
                    .HasName("PK_dbo.FieldItems");

                entity.Property(e => e.Cols).HasDefaultValueSql("((2))");

                entity.Property(e => e.CrmMapping).HasColumnName("CRM_Mapping");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.FormList)
                    .WithMany(p => p.FieldItems)
                    .HasForeignKey(d => d.FormListId)
                    .HasConstraintName("FK_dbo.FieldItems_dbo.FormLists_FormListId");
            });

            modelBuilder.Entity<FormLists>(entity =>
            {
                entity.HasKey(e => e.FormListId)
                    .HasName("PK_dbo.FormLists");

                entity.Property(e => e.FormListId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.BackgroundImage).HasMaxLength(128);

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.IsTransparent)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'false')");

                entity.Property(e => e.NotificationEmailFieldSubmitter).HasDefaultValueSql("((0))");

                entity.Property(e => e.NotificationEmailFieldSubmitterTemplate).HasDefaultValueSql("((0))");

                entity.Property(e => e.NotificationEmailTemplate).HasDefaultValueSql("((0))");

                entity.Property(e => e.RedirectUrl).HasColumnName("RedirectURL");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.TransparentValue).HasDefaultValueSql("((5))");

                entity.Property(e => e.UserId).IsRequired();

                entity.Property(e => e.ValidateLanguage).HasMaxLength(50);
            });

            modelBuilder.Entity<FormSubmissions>(entity =>
            {
                entity.HasKey(e => e.SubmissionId)
                    .HasName("PK_dbo.FormSubmissions");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.ReferrerUrl).HasColumnName("ReferrerURL");

                entity.Property(e => e.RelatedToContactId).HasMaxLength(50);

                entity.HasOne(d => d.FormList)
                    .WithMany(p => p.FormSubmissions)
                    .HasForeignKey(d => d.FormListId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_dbo.FormSubmissions_dbo.FormLists_FormListId");
            });

            modelBuilder.Entity<FormTemplateFieldElements>(entity =>
            {
                entity.HasKey(e => e.FieldItemElementId)
                    .HasName("PK_dbo.FormTemplateFieldElements");

                entity.HasOne(d => d.FieldItem)
                    .WithMany(p => p.FormTemplateFieldElements)
                    .HasForeignKey(d => d.FieldItemId)
                    .HasConstraintName("FK_dbo.FormTemplateFieldElements_dbo.FormTemplateFields_TemplateFieldId");
            });

            modelBuilder.Entity<FormTemplateFields>(entity =>
            {
                entity.HasKey(e => e.FieldItemId)
                    .HasName("PK_dbo.FormTemplateFields");

                entity.Property(e => e.Cols).HasDefaultValueSql("((2))");

                entity.Property(e => e.CrmMapping).HasColumnName("CRM_Mapping");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.FormTemplateFields)
                    .HasForeignKey(d => d.TemplateId)
                    .HasConstraintName("FK_dbo.FormTemplateFields_dbo.FormTemplates_TemplateId");
            });

            modelBuilder.Entity<FormTemplates>(entity =>
            {
                entity.HasKey(e => e.TemplateId)
                    .HasName("PK_dbo.FormTemplates");

                entity.Property(e => e.TemplateId).ValueGeneratedNever();

                entity.Property(e => e.BackgroundImage).HasMaxLength(128);

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.IsTransparent)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'false')");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.TransparentValue).HasDefaultValueSql("((5))");

                entity.Property(e => e.UserId).IsRequired();
            });

            modelBuilder.Entity<GlobalSettings>(entity =>
            {
                entity.Property(e => e.Dateformat).HasColumnName("dateformat");

                entity.Property(e => e.InvoiceEmailTo).HasColumnName("invoiceEmailTo");

                entity.Property(e => e.Language).HasColumnName("language");

                entity.Property(e => e.Timezone).HasColumnName("timezone");

                entity.Property(e => e.ViewedCrmtour).HasColumnName("ViewedCRMTour");
            });

            modelBuilder.Entity<Invoices>(entity =>
            {
                entity.HasKey(e => e.InvoiceId)
                    .HasName("PK_dbo.Invoices");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.NextPaymentAttempt).HasColumnType("datetime");

                entity.Property(e => e.PeriodEnd).HasColumnType("datetime");

                entity.Property(e => e.PeriodStart).HasColumnType("datetime");

                entity.Property(e => e.StripeCustomerId).HasMaxLength(50);

                entity.Property(e => e.StripeId).HasMaxLength(50);

                entity.Property(e => e.UserId).HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<LogEmails>(entity =>
            {
                entity.HasKey(e => e.LogEmailId)
                    .HasName("PK_dbo.LogEmails");

                entity.Property(e => e.Cid).HasMaxLength(50);

                entity.Property(e => e.SendDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<LogGenerals>(entity =>
            {
                entity.HasKey(e => e.LogGeneralId)
                    .HasName("PK_dbo.LogGenerals");

                entity.Property(e => e.LogGeneralId).ValueGeneratedNever();

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.DateTimeSubmitted)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('1900-01-01T00:00:00.000')");

                entity.Property(e => e.Ipaddress).HasColumnName("IPAddress");

                entity.HasOne(d => d.FormList)
                    .WithMany(p => p.LogGenerals)
                    .HasForeignKey(d => d.FormListId)
                    .HasConstraintName("FK_dbo.LogGenerals_dbo.FormLists_FormListId");
            });

            modelBuilder.Entity<LogInteractWithForms>(entity =>
            {
                entity.HasKey(e => e.LogInteractWithFormId)
                    .HasName("PK_dbo.LogInteractWithForms");

                entity.HasOne(d => d.LogGeneral)
                    .WithMany(p => p.LogInteractWithForms)
                    .HasForeignKey(d => d.LogGeneralId)
                    .HasConstraintName("FK_dbo.LogInteractWithForms_dbo.LogGenerals_LogGeneralId");
            });

            modelBuilder.Entity<LogLastLogins>(entity =>
            {
                entity.HasKey(e => e.LogLastLoginId)
                    .HasName("PK_dbo.LogLastLogins");

                entity.Property(e => e.LoginDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<LogNotificationEmails>(entity =>
            {
                entity.HasKey(e => e.LogNotificationEmailId)
                    .HasName("PK_dbo.LogNotificationEmails");

                entity.Property(e => e.PeriodEnd).HasColumnType("datetime");

                entity.Property(e => e.PeriodStart).HasColumnType("datetime");

                entity.Property(e => e.SendDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<LogTimes>(entity =>
            {
                entity.HasKey(e => e.LogTimeId)
                    .HasName("PK_dbo.LogTimes");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.HasOne(d => d.LogGeneral)
                    .WithMany(p => p.LogTimes)
                    .HasForeignKey(d => d.LogGeneralId)
                    .HasConstraintName("FK_dbo.LogTimes_dbo.LogGenerals_LogGeneralId");
            });

            modelBuilder.Entity<MembershipTypes>(entity =>
            {
                entity.HasKey(e => e.MembershipTypeId)
                    .HasName("PK_dbo.MembershipTypes");
            });

            modelBuilder.Entity<PayPalAccounts>(entity =>
            {
                entity.HasKey(e => e.PayPalId)
                    .HasName("PK_dbo.PayPalAccounts");

                entity.Property(e => e.Business)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.CurrencyCode).HasColumnName("Currency_Code");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.InvoiceNo).HasColumnName("Invoice_No");

                entity.Property(e => e.NotifyUrl).HasColumnName("Notify_URL");

                entity.HasOne(d => d.FormList)
                    .WithMany(p => p.PayPalAccounts)
                    .HasForeignKey(d => d.FormListId)
                    .HasConstraintName("FK_dbo.PayPalAccounts_dbo.FormLists_FormListId");
            });

            modelBuilder.Entity<Reports>(entity =>
            {
                entity.HasKey(e => e.ReportId);

                entity.Property(e => e.ReportId).ValueGeneratedNever();

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.Color).HasMaxLength(50);

                entity.Property(e => e.Country).HasMaxLength(50);

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.DateEnd).HasColumnType("datetime");

                entity.Property(e => e.DateStart).HasColumnType("datetime");

                entity.Property(e => e.DeletedDate).HasColumnType("datetime");

                entity.Property(e => e.ReportType).HasMaxLength(50);
            });

            modelBuilder.Entity<Rules>(entity =>
            {
                entity.HasKey(e => e.RuleId)
                    .HasName("PK_dbo.Rules");

                entity.HasOne(d => d.FormList)
                    .WithMany(p => p.Rules)
                    .HasForeignKey(d => d.FormListId)
                    .HasConstraintName("FK_dbo.Rules_dbo.FormLists_FormListId");
            });

            modelBuilder.Entity<SavedFormSubmissions>(entity =>
            {
                entity.HasKey(e => e.SubmissionId)
                    .HasName("PK_dbo.SavedFormSubmissions");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.ReferrerUrl).HasColumnName("ReferrerURL");

                entity.HasOne(d => d.FormList)
                    .WithMany(p => p.SavedFormSubmissions)
                    .HasForeignKey(d => d.FormListId)
                    .HasConstraintName("FK_dbo.SavedFormSubmissions_dbo.FormLists_FormListId");
            });

            modelBuilder.Entity<SavedSubmitValues>(entity =>
            {
                entity.HasKey(e => e.SubmitValuesId)
                    .HasName("PK_dbo.SavedSubmitValues");

                entity.HasOne(d => d.Submission)
                    .WithMany(p => p.SavedSubmitValues)
                    .HasForeignKey(d => d.SubmissionId)
                    .HasConstraintName("FK_dbo.SavedSubmitValues_dbo.SavedFormSubmissions_SubmissionId");
            });

            modelBuilder.Entity<SubmissionNotes>(entity =>
            {
                entity.HasKey(e => e.SubmissionNoteId)
                    .HasName("PK_dbo.SubmissionNotes");

                entity.Property(e => e.SubmissionNoteId).ValueGeneratedNever();

                entity.Property(e => e.DateAdded).HasColumnType("datetime");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.Submission)
                    .WithMany(p => p.SubmissionNotes)
                    .HasForeignKey(d => d.SubmissionId)
                    .HasConstraintName("FK_dbo.SubmissionNotes_dbo.FormSubmissions_SubmissionId");
            });

            modelBuilder.Entity<SubmitProductOrders>(entity =>
            {
                entity.HasKey(e => e.SubmitValuesId)
                    .HasName("PK_dbo.SubmitProductOrders");

                entity.HasOne(d => d.Submission)
                    .WithMany(p => p.SubmitProductOrders)
                    .HasForeignKey(d => d.SubmissionId)
                    .HasConstraintName("FK_dbo.SubmitProductOrders_dbo.FormSubmissions_SubmissionId");
            });

            modelBuilder.Entity<SubmitValues>(entity =>
            {
                entity.HasOne(d => d.Submission)
                    .WithMany(p => p.SubmitValues)
                    .HasForeignKey(d => d.SubmissionId)
                    .HasConstraintName("FK_dbo.SubmitValues_dbo.FormSubmissions_SubmissionId");
            });

            modelBuilder.Entity<SubscriptionPlans>(entity =>
            {
                entity.Property(e => e.FriendlyId).IsRequired();

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.StripeAccountId).HasColumnName("StripeAccount_Id");

                entity.HasOne(d => d.StripeAccount)
                    .WithMany(p => p.SubscriptionPlans)
                    .HasForeignKey(d => d.StripeAccountId)
                    .HasConstraintName("FK_dbo.SubscriptionPlans_dbo.StripeAccounts_StripeAccount_Id");
            });

            modelBuilder.Entity<SubscriptionUsages>(entity =>
            {
                entity.Property(e => e.ApplicationUserName).HasMaxLength(128);

                entity.Property(e => e.PeriodEnd).HasColumnType("datetime");

                entity.Property(e => e.PeriodStart).HasColumnType("datetime");
            });

            modelBuilder.Entity<Subscriptions>(entity =>
            {
                entity.Property(e => e.End).HasColumnType("datetime");

                entity.Property(e => e.PaidEnd).HasColumnType("datetime");

                entity.Property(e => e.PaidStart).HasColumnType("datetime");

                entity.Property(e => e.Start).HasColumnType("datetime");

                entity.Property(e => e.StripeAccountId).HasColumnName("StripeAccount_Id");

                entity.Property(e => e.StripeId).HasMaxLength(50);

                entity.Property(e => e.TrialEnd).HasColumnType("datetime");

                entity.Property(e => e.TrialStart).HasColumnType("datetime");

                entity.HasOne(d => d.StripeAccount)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(d => d.StripeAccountId)
                    .HasConstraintName("FK_dbo.Subscriptions_dbo.StripeAccounts_StripeAccount_Id");

                entity.HasOne(d => d.SubscriptionPlan)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(d => d.SubscriptionPlanId)
                    .HasConstraintName("FK_dbo.Subscriptions_dbo.SubscriptionPlans_SubscriptionPlanId");
            });

            modelBuilder.Entity<TemplateCalculations>(entity =>
            {
                entity.HasKey(e => e.TemplateCalculationId)
                    .HasName("PK_dbo.TemplateCalculations");

                entity.HasOne(d => d.FormList)
                    .WithMany(p => p.TemplateCalculations)
                    .HasForeignKey(d => d.FormListId)
                    .HasConstraintName("FK_dbo.TemplateCalculations_dbo.FormTemplates_TemplateId");
            });

            modelBuilder.Entity<TemplateRules>(entity =>
            {
                entity.HasKey(e => e.TemplateRuleId)
                    .HasName("PK_dbo.TemplateRules");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.TemplateRules)
                    .HasForeignKey(d => d.TemplateId)
                    .HasConstraintName("FK_dbo.TemplateRules_dbo.FormTemplates_TemplateId");
            });

            modelBuilder.Entity<UserGroups>(entity =>
            {
                entity.HasKey(e => e.UserGroupId)
                    .HasName("PK_dbo.UserGroups");

                entity.Property(e => e.DateAdded).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__UserProf__1788CC4CAA65B584");

                entity.HasIndex(e => e.UserName)
                    .HasName("UQ__UserProf__C9F2845625BA4260")
                    .IsUnique();

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(56);
            });

            modelBuilder.Entity<UserProfileExtra>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("userId");
            });

            modelBuilder.Entity<WebpagesMembership>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__webpages__1788CC4C050687A1");

                entity.ToTable("webpages_Membership");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.ConfirmationToken).HasMaxLength(128);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.IsConfirmed).HasDefaultValueSql("((0))");

                entity.Property(e => e.LastPasswordFailureDate).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.PasswordChangedDate).HasColumnType("datetime");

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.PasswordVerificationToken).HasMaxLength(128);

                entity.Property(e => e.PasswordVerificationTokenExpirationDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<WebpagesOauthMembership>(entity =>
            {
                entity.HasKey(e => new { e.Provider, e.ProviderUserId })
                    .HasName("PK__webpages__F53FC0ED4F0A13B0");

                entity.ToTable("webpages_OAuthMembership");

                entity.Property(e => e.Provider).HasMaxLength(30);

                entity.Property(e => e.ProviderUserId).HasMaxLength(100);
            });

            modelBuilder.Entity<WebpagesRoles>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("PK__webpages__8AFACE1ABBA23C88");

                entity.ToTable("webpages_Roles");

                entity.HasIndex(e => e.RoleName)
                    .HasName("UQ__webpages__8A2B6160858B8B26")
                    .IsUnique();

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<WebpagesUsersInRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("PK__webpages__AF2760AD49E662AB");

                entity.ToTable("webpages_UsersInRoles");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.WebpagesUsersInRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_RoleId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.WebpagesUsersInRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_UserId");
            });
        }
    }
}
