using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dajeej.Data;
using Dajeej.Models;
using Microsoft.AspNetCore.Authorization;

namespace Dajeej.Areas.Admin.Pages.Items
{

    [Authorize(Roles = "Admin")]

    public class ItemDetailsModel : PageModel
    {
        private readonly Dajeej.Data.DajeejContext _context;

        public ItemDetailsModel(Dajeej.Data.DajeejContext context)
        {
            _context = context;
        }

        public Dajeej.Models.Item Items { get; set; }
        public static int shopId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Items = await _context.Items
                .Include(i => i.Category)
                
                .Include(i => i.Shop)
                .Include(i => i.SubCategory).FirstOrDefaultAsync(m => m.ItemId == id);

            if (Items == null)
            {
                return NotFound();
            }
          
            return Page();
        }
    }
}
