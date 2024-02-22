using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dajeej.Data;
using Dajeej.Models;
using System.Collections.Generic;
using System.Linq;
namespace Dajeej.Areas.Admin.Pages.SystemConfigration
{
    public class FeedBackDetailsModel : PageModel
    {
        private readonly DajeejContext _context;
        public ContactUs Msg;
        public FeedBackDetailsModel(DajeejContext context)
        {
            _context = context;

        }
        public void OnGet(int id)
        {
            Msg = _context.ContactUs.FirstOrDefault(e => e.ContactId == id);
        }
    }
}
