using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dajeej.Models
{
    public class Subscription
    {
        public int SubscriptionId { get; set; }
        public int? ShopId { get; set; }
        public int? PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PaymentID { get; set; }
        public double Price { get; set; }
        [JsonIgnore]
        public virtual Plan Plan { get; set; }
        [JsonIgnore]
        public virtual Shop Shop { get; set; }
        public bool Active { get; set; }
    }
}
