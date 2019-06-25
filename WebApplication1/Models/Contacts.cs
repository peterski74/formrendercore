using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class Contacts
    {
        public Contacts()
        {
            ContactNotes = new HashSet<ContactNotes>();
        }

        public Guid ContactId { get; set; }
        public string UserId { get; set; }
        public string CrmTitle { get; set; }
        public string CrmFirstName { get; set; }
        public string CrmLastName { get; set; }
        public string CrmAccountName { get; set; }
        public string CrmAccountType { get; set; }
        public string CrmPhone { get; set; }
        public string CrmMobile { get; set; }
        public bool CrmDoNotCall { get; set; }
        public string CrmEmail { get; set; }
        public string CrmWebsite { get; set; }
        public string CrmDescription { get; set; }
        public string CrmStreet { get; set; }
        public string CrmSuburb { get; set; }
        public string CrmState { get; set; }
        public string CrmPostcode { get; set; }
        public string CrmCountry { get; set; }
        public string CrmLeadSource { get; set; }
        public string CrmOpportunity { get; set; }
        public int CrmAssignedTo { get; set; }
        public DateTime CrmDateAdded { get; set; }
        public string Priority { get; set; }
        public bool Deleted { get; set; }
        public bool Viewed { get; set; }
        public bool Starred { get; set; }
        public string DeletedDate { get; set; }
        public string RelatedToSubmissionId { get; set; }

        public virtual ICollection<ContactNotes> ContactNotes { get; set; }
    }
}
