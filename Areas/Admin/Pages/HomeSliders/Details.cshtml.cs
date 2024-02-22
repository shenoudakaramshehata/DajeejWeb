using System;
using System.Collections.Generic;
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
    public class DetailsModel : PageModel
    {
        private DajeejContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;

        public DetailsModel(DajeejContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
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
        
    }
}
