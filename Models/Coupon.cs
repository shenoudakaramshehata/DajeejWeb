using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dajeej.Models
{
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }
        [Required]
        public string Serial { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime IssueDate { get; set; }
        public double? Amount { get; set; }
        public int CouponTypeId { get; set; }
        public bool Used { get; set; }
        [JsonIgnore]
        public virtual CouponType CouponType { get; set; }
    }
}