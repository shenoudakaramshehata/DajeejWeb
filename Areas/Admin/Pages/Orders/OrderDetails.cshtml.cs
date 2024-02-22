using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dajeej.Data;
using Dajeej.Models;
using Microsoft.AspNetCore.Authorization;

namespace Dajeej.Areas.Admin.Pages.Orders
{
    //[Authorize(Roles = "Admin")]

    public class OrderDetailsModel : PageModel
    {
        private readonly DajeejContext _context;

        public OrderDetailsModel(DajeejContext context)
        {
            _context = context;
        }

        public Models.ViewModels.OrderDetailsViewModel OrderDetailsViewModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

             OrderDetailsViewModel = new Models.ViewModels.OrderDetailsViewModel();

            OrderDetailsViewModel.Order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.PaymentMehod)
                .Include(o => o.CustomerAddress)
                .Include(o => o.Coupon)
                .Include(o => o.CouponType)
                .Include(o => o.Shop).FirstOrDefaultAsync(m => m.OrderId == id);

            if (OrderDetailsViewModel.Order == null)
            {
                return NotFound();
            }


            OrderDetailsViewModel.OrderItem = _context.OrderItems.Include(s => s.Item).Where(s => s.OrderId == id).ToList();

            return Page();
        }
    }
}
