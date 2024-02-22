using System.ComponentModel.DataAnnotations;
namespace Dajeej.Models
{
    public class EntityTypeNotify
    {
        [Key]
        public int EntityTypeNotifyId { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
    }
}
