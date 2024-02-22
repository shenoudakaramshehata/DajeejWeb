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
using Microsoft.AspNetCore.Hosting;
using NToastNotify;
using System.IO;

namespace Dajeej.Areas.Admin.Pages.Shop
{
    

    public class DeleteShopModel : PageModel
    {
        private readonly DajeejContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;

        public DeleteShopModel(DajeejContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;

        }

        [BindProperty]
        public Dajeej.Models.Shop Shop { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Shop = await _context.Shops
                
                
                .Include(s => s.Country)
                .FirstOrDefaultAsync(m => m.ShopId == id);

            if (Shop == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            try
            {


                if (id == null)
                {
                    return NotFound();
                }

                Shop = await _context.Shops.Include(e => e.Country)
                    .FirstOrDefaultAsync(e => e.ShopId == id);

                if (Shop != null)
                {
                    if (_context.Items.Any(c => c.ShopId == id))
                    {
                        _toastNotification.AddErrorToastMessage("You can not delete this Shop Because There Are Items Related This Shop");

                        return Page();

                    }
                    var subs = _context.Subscriptions.Where(e => e.ShopId == id);
                    if (subs != null)
                    {
                        _context.Subscriptions.RemoveRange(subs);
                    }
                    _context.Shops.Remove(Shop);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Shop Deleted successfully");
                    var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, Shop.Pic);
                    if (System.IO.File.Exists(ImagePath))
                    {
                        System.IO.File.Delete(ImagePath);
                    }
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Shop Not Found");

                }
            }
            catch(Exception)
            {
                _toastNotification.AddErrorToastMessage("Something Went Error");
            }
            return Redirect("/admin/Shop");
        }
    }
}
