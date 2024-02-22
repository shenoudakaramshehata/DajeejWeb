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

namespace Dajeej.Areas.Admin.Pages.HomeSliders
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
        public Slider slider { get; set; }
       
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            slider = _context.Sliders.Where(c => c.SliderId == id).FirstOrDefault();
            if (slider == null)
            {
                return Redirect("../SomethingwentError");
            }
            

            return Page();
        }
        public IActionResult OnPost(int? id)
        
        {
            

            try
            {
                var model = _context.Sliders.Where(c => c.SliderId == id).FirstOrDefault();
                if (model==null)
                {
                    return Redirect("../SomethingwentError");
                }
                
                var uniqeFileName = "";

                if (Response.HttpContext.Request.Form.Files.Count() > 0)
                {
                    string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "Images/Slider/");
                    string ext = Path.GetExtension(Response.HttpContext.Request.Form.Files[0].FileName);
                    uniqeFileName = Guid.NewGuid().ToString("N") + ext;
                    string uploadedImagePath = Path.Combine(uploadFolder, uniqeFileName);
                    using (FileStream fileStream = new FileStream(uploadedImagePath, FileMode.Create))
                    {
                        Response.HttpContext.Request.Form.Files[0].CopyTo(fileStream);
                    }
                    var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, "Images/Slider/" + model.Pic);
                    if (System.IO.File.Exists(ImagePath))
                    {
                        System.IO.File.Delete(ImagePath);
                    }
                    model.Pic = uniqeFileName;
                }
                
                model.IsActive = slider.IsActive;
                model.OrderIndex= slider.OrderIndex;
                model.CountryId = slider.CountryId;

                _context.Attach(model).State = EntityState.Modified;
                 _context.SaveChanges();
            }
            catch (Exception)
            {
                return Redirect("../SomethingwentError");
            }

            return Redirect("./Index");

        }
    }
}
