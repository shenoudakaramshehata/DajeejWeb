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

namespace Dajeej.Areas.Admin.Pages.Categories
{
    public class AddModel : PageModel
    {

        private readonly IWebHostEnvironment _host;
        private DajeejContext _context;

        private readonly IToastNotification _toastNotification;


        public AddModel(DajeejContext context,IWebHostEnvironment host, IToastNotification toastNotification)
        {
            _context = context;
            _host = host;
            _toastNotification = toastNotification;
        }
        public void OnGet()
        {
           
        }
        public IActionResult OnPost(Category model)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try {

                
                    var uniqeFileName = "";

                if (Response.HttpContext.Request.Form.Files.Count() > 0)
                {
                    string uploadFolder = Path.Combine(_host.WebRootPath, "Images/Category");

                    string ext = Path.GetExtension(Response.HttpContext.Request.Form.Files[0].FileName);

                    uniqeFileName = Guid.NewGuid().ToString("N") + ext;

                    string uploadedImagePath = Path.Combine(uploadFolder, uniqeFileName);

                    using (FileStream fileStream = new FileStream(uploadedImagePath, FileMode.Create))
                    {
                        Response.HttpContext.Request.Form.Files[0].CopyTo(fileStream);
                    }
                    model.CategoryPic = uniqeFileName;





                    var uniqeFileNameIcon = "";


                    string uploadFolder2 = Path.Combine(_host.WebRootPath, "Images/Category");

                    string ext2 = Path.GetExtension(Response.HttpContext.Request.Form.Files[1].FileName);

                    uniqeFileNameIcon = Guid.NewGuid().ToString("N") + ext2;

                    string uploadedImagePath2 = Path.Combine(uploadFolder2, uniqeFileNameIcon);

                    using (FileStream fileStream = new FileStream(uploadedImagePath2, FileMode.Create))
                    {
                        Response.HttpContext.Request.Form.Files[1].CopyTo(fileStream);
                    }
                    model.CategoryIcon = uniqeFileNameIcon;

                }
                        _context.Categories.Add(model);
                        _context.SaveChanges();
                    
                }
            
        catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");
            }


            return Redirect("Index");
        }

    }
}
