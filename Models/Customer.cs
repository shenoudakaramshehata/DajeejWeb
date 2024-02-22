using System;
using System.ComponentModel.DataAnnotations;

namespace Dajeej.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public DateTime RegisterDate { get; set; }
        public string Pic { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public int NumberOfPoints { get; set; }
        
    }
}
