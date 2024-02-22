using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dajeej.Data;
using Dajeej.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Dajeej.Areas.Admin.Pages.Coupon
{
    public class DeleteModel : PageModel
    {
        private DajeejContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;

        public DeleteModel(DajeejContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;

        }
        [BindProperty]
        public Dajeej.Models.Coupon coupon { get; set; }



        public async Task<IActionResult> OnGetAsync(int id)
        {

            try
            {
                coupon = await _context.Coupons.Include(e=>e.CouponType).FirstOrDefaultAsync(m => m.CouponId == id);

                if (coupon == null)
                {
                    return Redirect("../NotFound");
                }
            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }



            return Page();
        }



        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                coupon = await _context.Coupons.Include(e => e.CouponType).FirstOrDefaultAsync(m => m.CouponId == id);
                if (coupon != null)
                {

                    _context.Coupons.Remove(coupon);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Coupon Deleted successfully");
                }
            }
            catch (Exception)

            {
                _toastNotification.AddErrorToastMessage("Something went wrong");

                return Page();

            }

            return RedirectToPage("./Index");
        }

    }
}
