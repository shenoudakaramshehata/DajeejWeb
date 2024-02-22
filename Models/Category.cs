using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dajeej.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string CategoryTLAR { get; set; }
        [Required]
        public string CategoryTLEN { get; set; }
        public string CategoryPic { get; set; }
        public string CategoryIcon { get; set; }
        public bool? IsActive { get; set; }
        public int? OrderIndex { get; set; }
        [JsonIgnore]
        public virtual ICollection<SubCategory>SubCategories { get; set; }
        [JsonIgnore]
        public virtual ICollection<Item> Items { get; set; }
    }
}
