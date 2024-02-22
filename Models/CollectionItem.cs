
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Dajeej.Models
{
    public class CollectionItem
    {
        [Key]
        public int CollectionItemId { get; set; }
        public int ItemId { get; set; }

        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }

        public int CollectionId { get; set; }
        [ForeignKey("CollectionId")]
        public Collection Collection { get; set; }
    }
}
