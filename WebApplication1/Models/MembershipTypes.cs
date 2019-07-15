using System;
using System.Collections.Generic;

namespace ngFormey.Web.Models
{
    public partial class MembershipTypes
    {
        public int MembershipTypeId { get; set; }
        public string Name { get; set; }
        public int AllowedSubmissions { get; set; }
        public int AllowedUploads { get; set; }
        public int AllowedUploadsPerForm { get; set; }
        public int AllowedUsers { get; set; }
    }
}