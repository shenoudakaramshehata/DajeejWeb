using Dajeej.Data;
using Dajeej.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NToastNotify;


namespace Dajeej.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DajeejContext _context;

        private readonly IToastNotification _toastNotification;
        [BindProperty]
        public ContactUs contact { get; set; }
        public IndexModel(DajeejContext context,ILogger<IndexModel> logger,IToastNotification toastNotification)
        {
            _logger = logger;
            _context = context;
            _toastNotification = toastNotification;
        }

        public void OnGet()
        {
           
        }
        public IActionResult OnPost()
        {
            try
            {
                contact.TransDate = System.DateTime.Now;
                _context.ContactUs.Add(contact);
                _context.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Youe Message are Sending Successfully");
                return Page();
            }
            catch
            {
                _toastNotification.AddErrorToastMessage("Something Went Error ..Try again Please!");
                return Redirect("/Contactus");
            }
        }
    }
}
