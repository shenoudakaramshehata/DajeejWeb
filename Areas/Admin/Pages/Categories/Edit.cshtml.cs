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
        public Category category { get; set; }



        public async Task<IActionResult> OnGetAsync(int id)
        {


            try
            {
                category = await _context.Categories.FirstOrDefaultAsync(m => m.CategoryId == id);
                if (category == null)
                {
                    return Redirect("../Error");
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
                var model = _context.Categories.Where(c => c.CategoryId == id).FirstOrDefault();
                if (model == null)
                {
                    return Page();
                }
                var uniqeFileName = "";

                if (Response.HttpContext.Request.Form.Files.Count() ==1)
                {
                    string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "Images/Category");
                    string ext = Path.GetExtension(Response.HttpContext.Request.Form.Files[0].FileName);
                    uniqeFileName = Guid.NewGuid().ToString("N") + ext;
                    string uploadedImagePath = Path.Combine(uploadFolder, uniqeFileName);
                    using (FileStream fileStream = new FileStream(uploadedImagePath, FileMode.Create))
                    {
                        Response.HttpContext.Request.Form.Files[0].CopyTo(fileStream);
                    }
                    model.CategoryPic = uniqeFileName;
                }
                var uniqeFileNameIcon = "";

                if (Response.HttpContext.Request.Form.Files.Count()==2)
                {
                    string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "Images/Category");
                    string ext = Path.GetExtension(Response.HttpContext.Request.Form.Files[1].FileName);
                    uniqeFileNameIcon = Guid.NewGuid().ToString("N") + ext;
                    string uploadedImagePath = Path.Combine(uploadFolder, uniqeFileNameIcon);
                    using (FileStream fileStream = new FileStream(uploadedImagePath, FileMode.Create))
                    {
                        Response.HttpContext.Request.Form.Files[1].CopyTo(fileStream);
                    }
                    model.CategoryIcon = uniqeFileNameIcon;
                }
                model.OrderIndex = category.OrderIndex;
                model.IsActive = category.IsActive;
                model.CategoryTLAR = category.CategoryTLAR;
                model.CategoryTLEN = category.CategoryTLEN;

                _context.Attach(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("category Edited successfully");

            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");

            }

            return RedirectToPage("./Index");
        }
    }
}
