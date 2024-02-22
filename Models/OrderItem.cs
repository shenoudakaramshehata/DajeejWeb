using Dajeej.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dajeej.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        public int? OrderId { get; set; }
        public int? ItemId { get; set; }
        public double ItemPrice { get; set; }
        public int ItemQuantity { get; set; }
        public double Total { get; set; }
        [JsonIgnore]
        public virtual Order Order { get; set; }
        [JsonIgnore]
        public virtual Item Item { get; set; }
        
    }
}
