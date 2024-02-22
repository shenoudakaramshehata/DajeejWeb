using System;
using System.ComponentModel.DataAnnotations;

namespace Dajeej.Models
{
    public class ShopVm
    {
        
        [Required(ErrorMessage = " Is Required")]
        public string ShopTLAR { get; set; }
        [Required(ErrorMessage = "Is Required")]
        public string ShopTLEN { get; set; }
        [Required(ErrorMessage = "Is Required")]
        public string DescriptionTLAR { get; set; }
        [Required(ErrorMessage = "Is Required")]
        public string DescriptionTLEN { get; set; }
        [Required(ErrorMessage = "Is Required")]
        public string Tele { get; set; }
        [Required(ErrorMessage = "Is Required")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Is Required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Is Required")]
        public string Instgram { get; set; }
        [Required(ErrorMessage = "Is Required")]
        public string ShopNo { get; set; }
       
        public int? OrderIndex { get; set; }
        public string Banner { get; set; }
        public string Pic { get; set; }
        [Required(ErrorMessage = "Is Required")]
        public double? DeliveryCost { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        [Required(ErrorMessage = "Is Required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Is Required")]
        public string Password { get; set; }
        public int CountryId { get; set; }
        public int PlanId { get; set; }
        public int? EntityTypeId { get; set; }
    
        

    }
}
