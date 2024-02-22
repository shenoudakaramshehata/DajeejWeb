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
using NToastNotify;

namespace Dajeej.Areas.Admin.Pages.HomeSliders
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
        public Slider slider { get; set; }
       

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                slider = _context.Sliders.Include(c=>c.Country).Where(c => c.SliderId == id).FirstOrDefault();
                if (slider == null)
                {
                    return Redirect("../SomethingwentError");
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
                var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, "Images/Slider/" +slider.Pic);


                slider = await _context.Sliders.FindAsync(id);
                if (slider != null)
                {
                    _context.Sliders.Remove(slider);
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

            return RedirectToPage("./Index");
        }

    }
}
