using System;
using System.ComponentModel.DataAnnotations;

namespace Dajeej.Models
{
    public partial class ContactUs
    {
        [Key]
        public int ContactId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string Msg { get; set; }
        public DateTime? TransDate { get; set; }
    }
}
