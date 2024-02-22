using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dajeej.Data;
using Dajeej.Models;
using Microsoft.EntityFrameworkCore;

namespace Dajeej.Areas.Admin.Pages.HomeSliders
{
    public class IndexModel : PageModel
    {

        private DajeejContext _context;
        public IndexModel(DajeejContext context)
        {
            _context = context;
        }
        [BindProperty(SupportsGet = true)]
        public bool ArLang { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Slider> SliderLst { get; set; }

        public void OnGet()
        {
            SliderLst = _context.Sliders.Include(s => s.Country).ToList();
            
        }
    }
}
