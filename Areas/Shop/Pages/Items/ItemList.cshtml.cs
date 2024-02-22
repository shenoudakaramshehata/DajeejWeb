using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dajeej.Areas.Shop.Pages.Items
{
    [Authorize(Roles = "Admin,Shop")]

    public class ItemListModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
