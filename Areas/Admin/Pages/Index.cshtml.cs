using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dajeej.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dajeej.Areas.Admin.Pages
{
    //[Authorize(Roles = "Admin")]

    public class IndexModel : PageModel
    {
        private readonly DajeejContext _context;

        public IndexModel(DajeejContext context)
        {
            _context = context;
        }
        [BindProperty(SupportsGet = true)]

        public string url { get; set; }
        public void OnGet()
        {
            url = $"{this.Request.Scheme}://{this.Request.Host}";

        }

    }
}
