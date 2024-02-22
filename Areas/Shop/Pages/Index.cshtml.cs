using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dajeej.Areas.Shop
{
    [Authorize(Roles = "Admin,Shop")]
    public class IndexModel : PageModel
    {
        
        public void OnGet()
        {
        }
    }
}
