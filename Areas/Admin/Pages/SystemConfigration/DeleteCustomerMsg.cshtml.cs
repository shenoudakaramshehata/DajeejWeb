using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dajeej.Data;
using Dajeej.Models;
using NToastNotify;

namespace Dajeej.Areas.Admin.SystemConfigration
{
    public class DeleteCustomerMsgModel : PageModel
    {
        private readonly Dajeej.Data.DajeejContext _context;
        private readonly IToastNotification _toastNotification;
        public DeleteCustomerMsgModel(Dajeej.Data.DajeejContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

        public ContactUs ContactUsMessages { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ContactUsMessages = await _context.ContactUs.FirstOrDefaultAsync(m => m.ContactId == id);

            if (ContactUsMessages == null)
            {
                return NotFound();
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                ContactUsMessages = await _context.ContactUs.FindAsync(id);

                if (ContactUsMessages != null)
                {
                    _context.ContactUs.Remove(ContactUsMessages);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Customer Message Deleted Successfully");
                }

            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something went wrong");
            }
           
            return Redirect("/Admin/SystemConfigration/UsersFeedBacks");
        }
    }
}

