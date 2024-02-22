using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Dajeej.Models
{
    
    public  class ShoppingCart
    {
        [Key]
        public int ShoppingCartId { get; set; }
        public int? CustomerId { get; set; }
        public int? ItemId { get; set; }
        public double ItemPrice { get; set; }
        public int ItemQty { get; set; }
        public double ItemTotal { get; set; }
        public double? Deliverycost { get; set; }
        [JsonIgnore]
        public virtual Customer Customer { get; set; }
        [JsonIgnore]
        public virtual Item Item { get; set; }
    }
}