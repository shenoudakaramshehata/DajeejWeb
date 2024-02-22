using Dajeej.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Dajeej.Models
{
    
    public class Order
    {
        
        [Key]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderSerial { get; set; }
        public int? CustomerId { get; set; }
        public int? CustomerAddressId { get; set; }
        public bool? IsDeliverd { get; set; }
        public string Notes { get; set; }
        public int? PaymentMethodId { get; set; }
        public int ShopId { get; set; }
        public double OrderTotal { get; set; }
        public double OrderDiscount { get; set; }
        public int? CouponId { get; set; }
        public int? CouponTypeId { get; set; }
        public double? CouponAmount { get; set; }
        public double? Deliverycost { get; set; }
        public double? OrderNet { get; set; }
        public bool IsPaid { get; set; }
        public int UniqeId { get; set; }
        
        public string PaymentID { get; set; }
        public DateTime? PostDate { get; set; }
        //public string Auth { get; set; }
        //public string Ref { get; set; }
        //public string Result { get; set; }
        //public string TranID { get; set; }
        //public string TrackID { get; set; }
        //public string Payment_type { get; set; }
        [JsonIgnore]
        public virtual Coupon Coupon { get; set; }
        [JsonIgnore]
        public virtual CouponType CouponType { get; set; }
        [JsonIgnore]
        public virtual Customer Customer { get; set; }
        [JsonIgnore]
        public virtual Shop Shop { get; set; }
        [JsonIgnore]
        public virtual CustomerAddress CustomerAddress { get; set; }
        [JsonIgnore]
        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMehod PaymentMehod { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderItem> OrderItems { get; set; }


    }
}