using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dajeej.Data;
using Dajeej.Models;
using Dajeej.Reports;
using Dajeej.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Dajeej.Areas.Admin.Pages.Reports
{
    [Authorize(Roles = "Admin")]
    public class RevenueModel : PageModel
    {
        public DajeejContext _context { get; }
        public RevenueModel(DajeejContext context)
        {
            _context = context;
        }

       public rptrevenue Report { get; set; }
        [BindProperty]
        public FilterModel filterModel { get; set; }

        public void OnGet()
        {
           Report = new rptrevenue();
        }
        public IActionResult OnPost()
        {

            List<OrderVm> ds = _context.Orders.Include(a => a.CustomerAddress).Include(a => a.Shop).Include(a => a.Customer).Include(a => a.PaymentMehod).Select(i => new OrderVm
            {
                OrderId = i.OrderId,
                OrderSerial = i.OrderSerial,
                
                CouponAmount = i.CouponAmount,
                CustomerName = i.Customer.NameAr,
                OrderNet = i.OrderNet,
                OrderDate = i.OrderDate,
                CustomerId = i.CustomerId,
                OrderDiscount = i.OrderDiscount,
                CouponId = i.CouponId,
                Deliverycost = i.Deliverycost,
                IsDeliverd = i.IsDeliverd == true ? "Deliverd" : "Not Deliverd",
                OrderTotal = i.OrderTotal,
                PaymentMethodId = i.PaymentMethodId,
                PaymentMethodName = i.PaymentMehod.PaymentMethodName,
                ShippingAddressId = i.CustomerAddressId,
                ispaid = i.IsPaid == true ? "Paid" : "Not Paid",
                ShippingAddressTitle = i.CustomerAddress.AddressNickname,
                ShopId = i.ShopId,
                ShopName = i.Shop.ShopTLAR,
                uniqeId = i.UniqeId,


            }).ToList();

            if (filterModel.ShopId != null)
            {
                ds = ds.Where(i => i.ShopId == filterModel.ShopId).ToList();
            }
            if (filterModel.ShippingAddressId != null)
            {
                ds = ds.Where(i => i.ShippingAddressId == filterModel.ShippingAddressId).ToList();
            }
            if (filterModel.radiobtn != null)
            {
                if (filterModel.radiobtn == "OnDay")
                {
                    if (filterModel.OnDay != null)
                    {
                        ds = ds.Where(i => i.OrderDate.Date == filterModel.OnDay.Value.Date).ToList();
                    }
                    else
                    {
                        ds = null;
                    }
                }
                else if (filterModel.radiobtn == "Period")
                {
                    if (filterModel.FromDate != null && filterModel.ToDate == null)
                    {
                        ds = null;
                    }
                    if (filterModel.FromDate == null && filterModel.ToDate != null)
                    {
                        ds = null;
                    }
                    if (filterModel.FromDate != null && filterModel.ToDate != null)

                    {
                        ds = ds.Where(i => i.OrderDate.Date >= filterModel.FromDate.Value.Date && i.OrderDate <= filterModel.ToDate.Value.Date).ToList();
                    }
                }
            }
            if(filterModel.radiobtn == null &&(filterModel.OnDay != null|| filterModel.FromDate != null || filterModel.ToDate != null ))
            {
                ds = null;
            }

            if (filterModel.ShopId == null && filterModel.ShippingAddressId == null && filterModel.FromDate == null && filterModel.ToDate == null && filterModel.radiobtn == null)
            {
                ds = null;
            }

            Report = new rptrevenue();
            Report.DataSource = ds;
            return Page();

        }
    }
}
