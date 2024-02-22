using Microsoft.AspNetCore.Identity;

namespace Dajeej.Data
{
    public class ApplicationUser : IdentityUser
    {
        public int EntityId { get; set; }
        public string Pic { get; set; }

    }
}
