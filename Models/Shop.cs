using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dajeej.Models
{
    public class Shop
    {
        [Key]
        public int ShopId { get; set; }
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
        public bool? IsActive { get; set; }
        public int? OrderIndex { get; set; }
        public string Banner { get; set; }
        public string Pic { get; set; }
        public DateTime? RegisterDate { get; set; }
        [Required(ErrorMessage = "Is Required")]
        public double? DeliveryCost { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        [Required(ErrorMessage ="Is Required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Is Required")]
        public string Password { get; set; }
        public int CountryId { get; set; }
        public int? EntityTypeId { get; set; }
        [JsonIgnore]
        public virtual Country Country { get; set; }
        [JsonIgnore]
        public virtual EntityType EntityType { get; set; }
        [JsonIgnore]
        public virtual ICollection<Subscription> Subscriptions { get; set; }
        [JsonIgnore]
        public virtual ICollection<ShopImage> ShopImage { get; set; }

    }
}
