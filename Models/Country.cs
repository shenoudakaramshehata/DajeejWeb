
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dajeej.Models
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }
        [Required]
        public string CountryTLAR { get; set; }
        [Required]
        public string CountryTLEN { get; set; }
        public string Pic { get; set; }
        public bool? IsActive { get; set; }
        public int? OrderIndex { get; set; }
        //[JsonIgnore]
        //public virtual ICollection<Plan>Plans { get; set; }
        //[JsonIgnore]
        //public virtual ICollection<Shop>Shops { get; set; }
    }
}
