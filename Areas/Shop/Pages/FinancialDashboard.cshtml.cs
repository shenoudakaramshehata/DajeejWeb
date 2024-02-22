using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dajeej.Areas.Shop.Pages
{

    [Authorize(Roles = "Shop")]
    public class FinancialDashboardModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
