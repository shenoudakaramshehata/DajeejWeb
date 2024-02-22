using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dajeej.Data;
using Dajeej.Models;

namespace Dajeej.Areas.Admin.Pages.Banners
{
    public class AddModel : PageModel
    {
        private DajeejContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public AddModel(DajeejContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        
        
        public void OnGet()
        {

        }
        public IActionResult OnPost(Banner model)
        {
            if (!ModelState.IsValid)
            {
                //return Page();
            }
            try
            {
                if (model.EntityTypeNotifyId == 1)
                {
                    model.EntityId = Request.Form["ItemId"];
                }
                if (model.EntityTypeNotifyId == 2)
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
                    model.Pic = uniqeFileName;
                }
                _context.Banners.Add(model);
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
