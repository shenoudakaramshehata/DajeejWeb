using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dajeej.Data;
using Dajeej.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Dajeej.Areas.Shop.Pages.shop
{
    public class IndexModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _host;
        private DajeejContext _context;
        private IHttpContextAccessor _httpContextAccessor;


        public IndexModel(DajeejContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db,
            IWebHostEnvironment host, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _host = host;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public Dajeej.Models.Shop Shop { get; set; }
        public void OnGet(int id)
        {
            //var InstructorID = _userManager.FindByNameAsync(User.Identity.Name).Result.EntityId;
            Shop = _context.Shops.Find(id);
            Shop.Email = _db.ApplicationUsers.FirstOrDefault(s => s.EntityId == id).Email;
        }
        public async Task<IActionResult> OnPostAsync(Dajeej.Models.Shop Shop)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var oldShop = _db.ApplicationUsers.FirstOrDefault(a => a.EntityId == Shop.ShopId);

            if (ModelState.IsValid)
            {

                var user = _db.ApplicationUsers.FirstOrDefault(s => s.EntityId == Shop.ShopId);

                if (user != null)
                {
                    user.Email = Shop.Email;

                    var uniqeFileName = "";

                    if (Response.HttpContext.Request.Form.Files.Count() > 0)
                    {
                        string uploadFolder = Path.Combine(_host.WebRootPath, "Images/Shop/");

                        uniqeFileName = Guid.NewGuid() + "_" + Response.HttpContext.Request.Form.Files[0].FileName;

                        string uploadedImagePath = Path.Combine(uploadFolder, uniqeFileName);

                        using (FileStream fileStream = new FileStream(uploadedImagePath, FileMode.Create))
                        {
                            Response.HttpContext.Request.Form.Files[0].CopyTo(fileStream);
                        }

                        Shop.Pic = "Images/Shop/"+uniqeFileName;
                        user.Pic = "Images/Shop/"+uniqeFileName;
                    }
                    else
                    {

                        var oldPictureIsDefault = _context.Shops.AsNoTracking().FirstOrDefault(s => s.ShopId == oldShop.EntityId).Pic.Contains("defaultPicture535");
                        if (oldPictureIsDefault)
                        {

                            DefaultAvatar def = new DefaultAvatar(_host);

                            Shop.Pic = def.CreateProfilePicture(Shop.ShopTLAR);

                        }
                        else
                        {

                            Shop.Pic = user.Pic;
                        }

                    }

                }

                _db.SaveChanges();
                var UpdatedShop = _context.Shops.Attach(Shop);
                UpdatedShop.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                
                await _context.SaveChangesAsync();

                return Redirect("myShop");
            }

            return Page();
        }

    }
}

