using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dajeej.Models
{
    public class Favourite
    {

        [Key]
        public int FavouriteId { get; set; }
        public int? CustomerId { get; set; }
        public int? ItemId { get; set; }
        [JsonIgnore]
        public virtual Customer Customer { get; set; }
        [JsonIgnore]
        public virtual Item Item { get; set; }
    }
}
