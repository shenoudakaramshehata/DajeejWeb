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

namespace Dajeej.Areas.Shop.Pages.Items
{
    [Authorize(Roles = "Admin,Shop")]

    public class DeleteItemModel : PageModel
    {
        private readonly DajeejContext _context;

        public DeleteItemModel(DajeejContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Dajeej.Models.Item Items { get; set; }

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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Items = await _context.Items.FindAsync(id);

            if (Items != null)
            {
                _context.Items.Remove(Items);
                await _context.SaveChangesAsync();
            }

            return Redirect("/shop/items/itemlist");
        }
    }
}
