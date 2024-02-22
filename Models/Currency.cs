using System.ComponentModel.DataAnnotations;

namespace Dajeej.Models
{
    public class Currency
    {
        [Key]
        public int CurrencyId { get; set; }
       
        [Required]
        public string CurrencyTLAR { get; set; }
        [Required]
        public string CurrencyTLEN { get; set; }
        public string CurrencyPic { get; set; }
        public bool? IsActive { get; set; }

        
    }
}
