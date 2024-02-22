using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dajeej.Data;
using Dajeej.Models;
using Dajeej.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Dajeej.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Dajeej.Areas.Shop.Pages.Reports
{
    [Authorize(Roles = "Admin,Shop")]
    public class ShopRevenueModel : PageModel
    {
        public DajeejContext _context { get; }
        UserManager<ApplicationUser> UserManger;
        public ShopRevenueModel(DajeejContext context, UserManager<ApplicationUser> userManger)
        {
            _context = context;
            UserManger = userManger;

        }

        public rptshoprevenue Report { get; set; }
        [BindProperty]
        public FilterModel filterModel { get; set; }

        public void OnGet()
        {
            Report = new rptshoprevenue();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await UserManger.FindByIdAsync(userid);
            List<OrderVm> ds = _context.Orders.Include(a => a.CustomerAddress).Include(a => a.Shop).Include(a => a.Customer).Include(a => a.PaymentMehod).Where(e=>e.ShopId== user.EntityId).Select(i => new OrderVm
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
                Picture=i.Shop.Pic,
               
                customerEmail=i.Customer.Email,
                customerPhone=i.Customer.Mobile,
                ShopEmail=i.Shop.Email,
                ShopMobile=i.Shop.Mobile,
                ShopAddress=i.Shop.Address,

                PaymentMethodId = i.PaymentMethodId,
                PaymentMethodName = i.PaymentMehod.PaymentMethodName,
                ShippingAddressId = i.CustomerAddressId,
                ispaid = i.IsPaid == true ? "Paid" : "Not Paid",

                ShippingAddressTitle = i.CustomerAddress.AddressNickname,
                ShopId = i.ShopId,
                ShopName = i.Shop.ShopTLAR,
                uniqeId = i.UniqeId,


            }).ToList();

            if (filterModel.CustomterId != null)
            {
                ds = ds.Where(i => i.CustomerId == filterModel.CustomterId).ToList();
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
            if (filterModel.radiobtn == null && (filterModel.OnDay != null || filterModel.FromDate != null || filterModel.ToDate != null))
            {
                ds = null;
            }

            if (filterModel.CustomterId == null && filterModel.ShippingAddressId == null && filterModel.FromDate == null && filterModel.ToDate == null && filterModel.radiobtn == null)
            {
                ds = null;
            }

            Report = new rptshoprevenue();
            Report.DataSource = ds;
            return Page();

        }
    }
}
