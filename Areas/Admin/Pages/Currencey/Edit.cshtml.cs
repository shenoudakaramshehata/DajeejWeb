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


namespace Dajeej.Areas.Admin.Pages.Currencey
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
        public Currency currency { get; set; }



        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {

                currency = await _context.Currencies.FirstOrDefaultAsync(m => m.CurrencyId == id);
                if (currency == null)
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

                var model = _context.Currencies.Where(c => c.CurrencyId == id).FirstOrDefault();
                if (model == null)
                {
                    return Redirect("../NotFound");
                }
                var uniqeFileName = "";

                if (Response.HttpContext.Request.Form.Files.Count() > 0)
                {

                    var ImagePath = Path.Combine(_hostEnvironment.WebRootPath, "Images/Currency/" + model.CurrencyPic);
                    if (System.IO.File.Exists(ImagePath))
                    {
                        System.IO.File.Delete(ImagePath);
                    }
                    string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "Images/Currency/");

                    string ext = Path.GetExtension(Response.HttpContext.Request.Form.Files[0].FileName);

                    uniqeFileName = Guid.NewGuid().ToString("N") + ext;

                    string uploadedImagePath = Path.Combine(uploadFolder, uniqeFileName);

                    using (FileStream fileStream = new FileStream(uploadedImagePath, FileMode.Create))
                    {
                        Response.HttpContext.Request.Form.Files[0].CopyTo(fileStream);
                    }
                    model.CurrencyPic = uniqeFileName;
                }
                model.IsActive = currency.IsActive;
                model.CurrencyTLAR = currency.CurrencyTLAR;
                model.CurrencyTLEN = currency.CurrencyTLEN;
                _context.Attach(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Currency Edited successfully");


            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");

            }

            return RedirectToPage("./Index");
        }
    }
}
