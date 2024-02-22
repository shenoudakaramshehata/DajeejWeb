using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Dajeej.Models
{
    public  class Slider
    {
        [Key]
        public int SliderId { get; set; }
        public string Pic { get; set; }
        public bool? IsActive { get; set; }
        [Required]
        public int? OrderIndex { get; set; }
        public int? CountryId { get; set; }
        [ForeignKey("CountryId")]
        [JsonIgnore]
        public Country Country { get; set; }
    }
}