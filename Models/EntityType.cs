using System.ComponentModel.DataAnnotations;
namespace Dajeej.Models
{
    public class EntityType
    {
        [Key]
        public int EntityTypeId{ get; set; }
        public string TitleAr{ get; set; }
        public string TitleEn{ get; set; }
    }
}
