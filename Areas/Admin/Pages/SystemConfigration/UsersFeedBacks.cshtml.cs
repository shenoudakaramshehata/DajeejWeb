using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dajeej.Data;
using Dajeej.Models;
using System.Collections.Generic;
using System.Linq;

namespace Dajeej.Areas.Admin.Pages.SystemConfigration
{
    public class UsersFeedBacksModel : PageModel
    {
        private readonly DajeejContext _context;
        public List<ContactUs> Msgs;
        public UsersFeedBacksModel(DajeejContext context)
        {
            _context = context;
           
        }
        public void OnGet()
        {
            Msgs = _context.ContactUs.ToList();
        }
    }
}
