using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dajeej.Data;
using Dajeej.Models;

namespace Dajeej.Areas.Admin.Pages.Banners
{
    public class EditModel : PageModel
    {
        private DajeejContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public EditModel(DajeejContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
      
        [BindProperty]
        public Banner banner { get; set; }
        [BindProperty]
        public int EntityId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            banner = _context.Banners.Where(c => c.BannerId == id).FirstOrDefault();
            if (banner == null)
            {
                return NotFound();
            }
        
                EntityId = int.Parse(banner.EntityId);
                banner.EntityId = null;


            return Page();
        }
        public IActionResult OnPost(int? id)
        
        {
            

            try
            {
                var model = _context.Banners.Where(c => c.BannerId == id).FirstOrDefault();
                if (model==null)
                {
                    return NotFound();
                }
                if (banner.EntityTypeNotifyId == 1)
                {
                    model.EntityId = Request.Form["ItemId"];
                }
                if (banner.EntityTypeNotifyId == 2)
                {
                    model.EntityId = Request.Form["ShopId"];
                }
              
                var uniqeFileName = "";

                if (Response.HttpContext.Request.Form.Files.Count() > 0)
                {
                    string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "Images/Banner/");
                    string ext = Path.GetExtension(Response.HttpContext.Request.Form.Files[0].FileName);
                    uniqeFileName = Guid.NewGuid().ToString("N") + ext;
                    string uploadedImagePath = Path.Combine(uploadFolder, uniqeFileName);
                    using (FileStream fileStream = new FileStream(uploadedImagePath, FileMode.Create))
                    {
                        Response.HttpContext.Request.Form.Files[0].CopyTo(fileStream);
                    }
                    var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, "Images/Banner/" + model.Pic);
                    if (System.IO.File.Exists(ImagePath))
                    {
                        System.IO.File.Delete(ImagePath);
                    }
                    model.Pic = uniqeFileName;
                }
                model.EntityTypeNotifyId = banner.EntityTypeNotifyId;
                model.IsActive = banner.IsActive;
                model.OrderIndex = banner.OrderIndex;
                model.CountryId = banner.CountryId;
                _context.Attach(model).State = EntityState.Modified;
                 _context.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }

            return Redirect("/admin/banners/index");

        }
    }
}
