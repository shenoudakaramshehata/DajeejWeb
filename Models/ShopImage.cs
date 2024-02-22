using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dajeej.Models
{
    public class ShopImage
    {
        [Key]
        public int ShopImagesId { get; set; }
        public string ImageName { get; set; }
        public int ShopId { get; set; }
        [JsonIgnore]
        public virtual Shop Shop { get; set; }
    }
}
