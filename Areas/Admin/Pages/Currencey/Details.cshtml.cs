using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dajeej.Data;
using Dajeej.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;


namespace Dajeej.Areas.Admin.Pages.Currencey
{
    public class DetailsModel : PageModel
    {
        private DajeejContext _context;


        private readonly IToastNotification _toastNotification;
        public DetailsModel(DajeejContext context, IToastNotification toastNotification)
        {
            _context = context;
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

    }
}
