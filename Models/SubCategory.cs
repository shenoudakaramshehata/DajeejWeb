using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dajeej.Models
{
    public class SubCategory
    {
        [Key]
        public int SubCategoryId { get; set; }
        public string SubCategoryTLAR { get; set; }
        public string SubCategoryTLEN { get; set; }
        public string SubCategoryPic { get; set; }
        public bool? IsActive { get; set; }
        public int? OrderIndex { get; set; }
        public int CategoryId { get; set; }
        [JsonIgnore]
        public virtual Category Category { get; set; }
        [JsonIgnore]
        public virtual ICollection<Item> Items { get; set; }


    }
}
