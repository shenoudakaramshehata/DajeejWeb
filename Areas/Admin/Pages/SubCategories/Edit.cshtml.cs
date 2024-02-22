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

namespace Dajeej.Areas.Admin.Pages.SubCategories
{
    public class EditModel : PageModel
    {
        private DajeejContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;

        public EditModel(DajeejContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;

        }
        [BindProperty]
        public SubCategory subcategory { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {

            try
            {


                subcategory = await _context.SubCategories.FirstOrDefaultAsync(m => m.SubCategoryId == id);
                if (subcategory == null)
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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var model = _context.SubCategories.Where(c => c.SubCategoryId == id).FirstOrDefault();
                if (model == null)
                {
                    return Redirect("../NorFound");

                }

                var uniqeFileName = "";

                if (Response.HttpContext.Request.Form.Files.Count() > 0)
                {

                    var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, "Images/SubCategories/" + model.SubCategoryPic);
                    if (System.IO.File.Exists(ImagePath))
                    {
                        System.IO.File.Delete(ImagePath);
                    }
                    string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "Images/SubCategories");

                    string ext = Path.GetExtension(Response.HttpContext.Request.Form.Files[0].FileName);

                    uniqeFileName = Guid.NewGuid().ToString("N") + ext;

                    string uploadedImagePath = Path.Combine(uploadFolder, uniqeFileName);

                    using (FileStream fileStream = new FileStream(uploadedImagePath, FileMode.Create))
                    {
                        Response.HttpContext.Request.Form.Files[0].CopyTo(fileStream);
                    }
                    model.SubCategoryPic = uniqeFileName;
                }

                model.IsActive = subcategory.IsActive;
                model.OrderIndex = subcategory.OrderIndex;
                model.SubCategoryTLAR = subcategory.SubCategoryTLAR;
                model.SubCategoryTLEN = subcategory.SubCategoryTLEN;
                model.CategoryId = subcategory.CategoryId;
                _context.Attach(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Subcategory Edited successfully");

            }
            catch (DbUpdateConcurrencyException)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");

            }

            return RedirectToPage("./Index");
        }
    }
}
