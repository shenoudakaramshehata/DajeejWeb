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

namespace Dajeej.Areas.Admin.Pages.Subscriptions
{
   
    public class DetailsModel : PageModel
    {
        private readonly DajeejContext _context;

        public DetailsModel(DajeejContext context)
        {
            _context = context;
        }

        public Dajeej.Models.Subscription Subscriptions { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Subscriptions = await _context.Subscriptions
                .Include(s => s.Plan)
                .Include(s => s.Shop).FirstOrDefaultAsync(m => m.SubscriptionId == id);

            if (Subscriptions == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
