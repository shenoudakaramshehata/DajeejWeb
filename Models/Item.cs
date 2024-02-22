using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dajeej.Models
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }
        [Required]
        public string ItemTitleAr { get; set; }

        [Required]
        public string ItemTitleEn { get; set; }
        public string ItemImage { get; set; }
        public string ItemDescriptionAr { get; set; }
        public string ItemDescriptionEn { get; set; }
        public double ItemPrice { get; set; }
        public bool? IsActive { get; set; }
        public int? OrderIndex { get; set; }
        public bool? OutOfStock { get; set; }
        public int CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int ShopId { get; set; }
        //public int CustomerId { get; set; }
        [JsonIgnore]
        public virtual Category Category { get; set; }
        [JsonIgnore]
        public virtual SubCategory SubCategory { get; set; }
        //public virtual Customer Customer { get; set; }
        [JsonIgnore]
        public virtual Shop Shop { get; set; }
        [JsonIgnore]
        public virtual ICollection<ItemImage> ItemImages { get; set; }

        
    }
}
