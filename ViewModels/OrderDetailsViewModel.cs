using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dajeej.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public OrderDetailsViewModel()
        {
            Order = new Order();
            OrderItem = new List<OrderItem>();
        }
        public Order Order { get; set; }
        public List<OrderItem> OrderItem { get; set; }
    }
}
