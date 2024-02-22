using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dajeej.Data;
using Dajeej.Models;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;

namespace Dajeej.Areas.Admin.Pages.Shop
{
    public class ShopDetailsModel : PageModel
    {
        private readonly DajeejContext _context;

        public ShopDetailsModel(DajeejContext context)
        {
            _context = context;
        }

        public Dajeej.Models.Shop Shop { get; set; }

        public JsonResult Subscriptions(DataSourceLoadOptions options)
        {
            
            return new JsonResult(DataSourceLoader.Load(_context.Subscriptions, options));
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Shop = await _context.Shops
                .Include(s => s.Subscriptions)
                .Include(s => s.Country)
                .FirstOrDefaultAsync(m => m.ShopId == id);

            if (Shop == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
