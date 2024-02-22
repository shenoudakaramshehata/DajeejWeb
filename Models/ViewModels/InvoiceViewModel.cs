using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dajeej.Models.ViewModels
{
    public class InvoiceViewModel
    {
        public InvoiceViewModel()
        {
            Order = new Order();
        }
        public Order Order { get; set; }
        public Shop Shop { get; set; }
        public Customer Customer { get; set; }
        public string Mobile { get; set; }
        public string InvoiceDate { get; set; }
        public string InvoiceNo { get; set; }
        public double TotalPrice { get; set; }
        public OrderItem OrderItem { get; set; }
    }
}
