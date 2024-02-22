using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.XtraReports.UI;
using Dajeej.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Dajeej.Areas.Shop.Pages.Reports
{
    [Authorize(Roles = "Admin,Shop")]
    public class OrderShopInvoiceModel : PageModel
    {
        private readonly Dajeej.Data.DajeejContext _context;
        public OrderShopInvoiceModel(Dajeej.Data.DajeejContext context)
        {
            _context = context;
        }



        public XtraReport1 Report { get; set; }

        public ActionResult OnGet(int id)
        {

            Report = new XtraReport1();

            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .FirstOrDefault(s => s.OrderId == id);

            var orderItems = _context.OrderItems.Include(o => o.Item).Where(s => s.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }


            var dd = _context.OrderItems.Where(o => o.OrderId == id)
                .Select(c => new InvoiceViewModel()
                {
                    Order = order,
                    OrderItem = _context.OrderItems.Include(s => s.Item).FirstOrDefault(s => s.OrderItemId == c.OrderItemId),
                    InvoiceDate = order.OrderDate.ToString(),
                    InvoiceNo = order.OrderId.ToString(),
                    Customer = order.Customer,
                    Shop = order.Shop
                }).ToList();
           

            Report.DataSource = dd;


            return Page();

        }

        public void OnPost()
        {
     

        }
    }
}
