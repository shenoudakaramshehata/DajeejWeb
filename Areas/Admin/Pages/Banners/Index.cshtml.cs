using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dajeej.Data;
using Dajeej.Models;
using Microsoft.EntityFrameworkCore;

namespace Dajeej.Areas.Admin.Pages.Banners
{
    public class IndexModel : PageModel
    {

        private DajeejContext _context;
        public IndexModel(DajeejContext context)
        {
            _context = context;
        }
        [BindProperty(SupportsGet = true)]
        public List<Banner> BannerLst { get; set; }

        public void OnGet()
        {
            BannerLst = _context.Banners.Include(e=>e.Country).ToList();
            foreach (var item in BannerLst)
            {
                if (item.EntityId==null)
                  continue;
                
                if (item.EntityTypeNotifyId==1)
                {
                    var id = Convert.ToInt32(item.EntityId);
                  
                    item.EntityId = _context.Items.FirstOrDefault(c => c.ItemId == id)?.ItemTitleAr;
                }
                if (item.EntityTypeNotifyId == 2)
                {
                    var id = Convert.ToInt32(item.EntityId);
                    item.EntityId = _context.Shops.FirstOrDefault(c => c.ShopId == id)?.ShopTLAR;
                }
               
            

            }
        }
    }
}
