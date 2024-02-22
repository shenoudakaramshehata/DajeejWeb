using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dajeej.Areas.Admin.Pages
{
    [Authorize(Roles = "Admin")]

    public class CollectionsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
