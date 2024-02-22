using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dajeej.Data;
using Dajeej.Models;
using NToastNotify;

namespace Dajeej.Areas.Admin.Pages.Banners
{
    public class DeleteModel : PageModel
    {

        private DajeejContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public DeleteModel (DajeejContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;

        }

        [BindProperty]
        public Banner banner { get; set; }
        [BindProperty]
        public int EntityId { get; set; }
        [BindProperty]
        public string EntityNameEn { get; set; }
        [BindProperty]
        public string EntityNameAr { get; set; }
        
        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                banner = _context.Banners.Include(c => c.EntityTypeNotify).Where(c => c.BannerId == id).FirstOrDefault();
                if (banner == null)
                {
                    return Redirect("../SomethingwentError");
                }

                if (banner.EntityTypeNotifyId == 1)
                {
                    var EntityId = Convert.ToInt32(banner.EntityId);

                    EntityNameEn = _context.Items.FirstOrDefault(c => c.ItemId == EntityId)?.ItemTitleAr;
                    EntityNameAr = _context.Items.FirstOrDefault(c => c.ItemId == EntityId)?.ItemTitleEn;
                }
                if (banner.EntityTypeNotifyId == 2)
                {
                    var EntityId = Convert.ToInt32(banner.EntityId);

                    EntityNameEn = _context.Shops.FirstOrDefault(c => c.ShopId == EntityId)?.ShopTLEN;
                    EntityNameAr = _context.Shops.FirstOrDefault(c => c.ShopId == EntityId)?.ShopTLAR;
                }
               

            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");

            }


            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, "Images/" +banner.Pic);


                banner = await _context.Banners.FindAsync(id);
                if (banner != null)
                {
                    _context.Banners.Remove(banner);
                    await _context.SaveChangesAsync();
                    _context.SaveChanges();
                    if (System.IO.File.Exists(ImagePath))
                    {
                        System.IO.File.Delete(ImagePath);
                    }
                    
                }
            }
            catch (Exception)

            {
                
                return Page();

            }

            return Redirect("/admin/banners/index");
        }

    }
}
