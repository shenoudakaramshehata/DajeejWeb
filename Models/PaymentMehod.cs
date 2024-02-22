using System.ComponentModel.DataAnnotations;

namespace Dajeej.Models
{
    public class PaymentMehod
    {
        [Key]
        public int PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public string PaymentMethodPic { get; set; }
    }
}
