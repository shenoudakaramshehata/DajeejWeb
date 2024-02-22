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

namespace Dajeej.Areas.Admin.Pages.Categories
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
        public Category category { get; set; }



        public async Task<IActionResult> OnGetAsync(int id)
        {

            try
            {
                category = await _context.Categories.FirstOrDefaultAsync(m => m.CategoryId == id);

                if (category == null)
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
                category = await _context.Categories.FindAsync(id);
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Category Deleted successfully");
                var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, "Images/Category/" + category.CategoryPic);
                if (System.IO.File.Exists(ImagePath))
                {
                    System.IO.File.Delete(ImagePath);
                }
                var ImagePathIcon = Path.Combine(_hostEnvironment.WebRootPath, "Images/Category/" + category.CategoryIcon);
                if (System.IO.File.Exists(ImagePathIcon))
                {
                    System.IO.File.Delete(ImagePathIcon);
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
