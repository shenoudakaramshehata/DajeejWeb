using System.ComponentModel.DataAnnotations;

namespace Dajeej.Models
{
    public class Plan
    {
        [Key]
        public int PlanId { get; set; }
        [Required]
        public string ArabicTitle { get; set; }
        [Required]
        public string EnglishTitle { get; set; }
        [Required]
        public int Period { get; set; }
        [Required]
        public int NoOfItems { get; set; }
        public double Price { get; set; }
        public bool VipCollection { get; set; }
        public bool Reports { get; set; }
        public bool Dashboard { get; set; }
        public bool AdzBanners { get; set; }
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }
        
        
    }
}
