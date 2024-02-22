using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dajeej.Data;
using Dajeej.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dajeej.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _host;
        private readonly ApplicationDbContext _db;
        private readonly DajeejContext _context;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment host,
            ApplicationDbContext db,
            DajeejContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _host = host;
            _db = db;
            _context = context;
        }

        public string Username { get; set; }

        [BindProperty]
        public IFormFile ProfilePicture { get; set; }

        [BindProperty]
        public IFormFile BannerPicture { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (ProfilePicture != null)
            {

                string pathOfAllPictures = Path.Combine(_host.WebRootPath, "images/CustomerImages/");

                if (user.Pic != null)
                {
                    string pathOfOldPic = Path.Combine(pathOfAllPictures, user.Pic);

                    if (System.IO.File.Exists(pathOfOldPic))
                    {
                        System.IO.File.Delete(pathOfOldPic);
                    }
                }


                string uniqePictureName = Guid.NewGuid() + "_" + ProfilePicture.FileName;
                string uploadedImagePath = Path.Combine(pathOfAllPictures, uniqePictureName);

                using (FileStream fileStream = new FileStream(uploadedImagePath, FileMode.Create))
                {
                    ProfilePicture.CopyTo(fileStream);
                }

                user.Pic = "images/CustomerImages/"+ uniqePictureName;
                _db.SaveChanges();

            }

            if (BannerPicture != null)
            {

                string pathOfAllPictures = Path.Combine(_host.WebRootPath, "images/Shop/");
                var shop = await _context.Shops.FindAsync(_userManager.FindByNameAsync(User.Identity.Name).Result.EntityId);

                if (shop.Banner != null)
                {
                    string pathOfOldPic = Path.Combine(pathOfAllPictures, shop.Banner);

                    if (System.IO.File.Exists(pathOfOldPic))
                    {
                        System.IO.File.Delete(pathOfOldPic);
                    }
                }


                string uniqePictureName = Guid.NewGuid() + "_" + BannerPicture.FileName;
                string uploadedImagePath = Path.Combine(pathOfAllPictures, uniqePictureName);

                using (FileStream fileStream = new FileStream(uploadedImagePath, FileMode.Create))
                {
                    BannerPicture.CopyTo(fileStream);
                }

                shop.Banner = uniqePictureName;
                _db.SaveChanges();
                _context.SaveChanges();

            }


            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
