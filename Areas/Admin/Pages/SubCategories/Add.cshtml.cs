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
using NToastNotify;

namespace Dajeej.Areas.Admin.Pages.SubCategories
{
    public class AddModel : PageModel
    {
        private DajeejContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IToastNotification _toastNotification;
        public AddModel(DajeejContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;

        }

        public void OnGet()
        {
        }
        public IActionResult OnPost(SubCategory model)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                if (model.CategoryId == 0)
                {
                    ModelState.AddModelError("CategoryError", "Category Is Required..");
                    return Page();
                }
                var uniqeFileName = "";
                string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "Images/SubCategories");


                if (Response.HttpContext.Request.Form.Files.Count() > 0)
                {

                    string ext = Path.GetExtension(Response.HttpContext.Request.Form.Files[0].FileName);

                    uniqeFileName = Guid.NewGuid().ToString("N") + ext;

                    string uploadedImagePath = Path.Combine(uploadFolder, uniqeFileName);

                    using (FileStream fileStream = new FileStream(uploadedImagePath, FileMode.Create))
                    {
                        Response.HttpContext.Request.Form.Files[0].CopyTo(fileStream);
                    }
                    model.SubCategoryPic = uniqeFileName;
                }
                _context.SubCategories.Add(model);
                _context.SaveChanges();
                

                _toastNotification.AddSuccessToastMessage("SubCategory Added successfully");
            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }

            return Redirect("./Index");

        }
    }
}
