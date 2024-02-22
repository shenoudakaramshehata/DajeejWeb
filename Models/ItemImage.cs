using System.Text.Json.Serialization;

namespace Dajeej.Models
{
    public class ItemImage
    {
        public int ItemImageId { get; set; }
        public string ImageName { get; set; }
        public int ItemId { get; set; }
        [JsonIgnore]
        public virtual Item Item { get; set; }

    }
}
