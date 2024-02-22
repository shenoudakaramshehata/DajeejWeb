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

namespace Dajeej.Areas.Admin.Pages.Plans
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
        public Plan plan { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {

            try
            {
                plan = await _context.Plans.Include(c => c.Country).FirstOrDefaultAsync(m => m.PlanId == id);
                if (plan == null)
                {
                    _toastNotification.AddErrorToastMessage("Plan Not Found");
                    return RedirectToPage("./List");

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
                plan = await _context.Plans.Include(c => c.Country).FirstOrDefaultAsync(m => m.PlanId == id);

                _context.Plans.Remove(plan);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Plan Deleted successfully");
            }
            catch (Exception)

            {
                _toastNotification.AddErrorToastMessage("Something went wrong");

                return Page();

            }

            return RedirectToPage("./List");
        }
    }
}
