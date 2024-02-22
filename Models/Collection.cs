
using System.ComponentModel.DataAnnotations;


namespace Dajeej.Models
{
    public class Collection
    {
        [Key]
        public int CollectionId { get; set; }
        [Required]
        public string CollectionTitleAr { get; set; }
        [Required]
        public string CollectionTitleEn { get; set; }

        [Required]
        public int Source { get; set; }
        public bool IsActive { get; set; }
    }
}
