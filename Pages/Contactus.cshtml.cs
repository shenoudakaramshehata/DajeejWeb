using Dajeej.Data;
using Dajeej.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;
using System.Threading.Tasks;

namespace Dajeej.Pages
{
    public class ContactusModel : PageModel
    {
        private DajeejContext _context;
        private readonly IToastNotification _toastNotification;
        [BindProperty]
        public ContactUs contact { get; set; }
        public ContactusModel(DajeejContext context, IToastNotification toastNotification)
        {
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
