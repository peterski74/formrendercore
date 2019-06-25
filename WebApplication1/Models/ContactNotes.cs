using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class ContactNotes
    {
        public Guid ContactNoteId { get; set; }
        public string Note { get; set; }
        public string UserId { get; set; }
        public DateTime DateAdded { get; set; }
        public Guid ContactId { get; set; }

        public virtual Contacts Contact { get; set; }
    }
}
