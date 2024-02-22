using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dajeej.Models
{
    public class CouponType
    {
        [Key]
        public int CouponTypeId { get; set; }
        public string Title { get; set; }
        [JsonIgnore]
        public virtual ICollection<Coupon> Coupon { get; set; }
    }
}