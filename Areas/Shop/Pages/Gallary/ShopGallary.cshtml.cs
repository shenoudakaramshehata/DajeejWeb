using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dajeej.Data;
using Dajeej.Models;
using Dajeej.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;


namespace Dajeej.Areas.Shop.Pages.Gallary
{
    public class ShopGallaryModel : PageModel
    {
        private readonly DajeejContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _host;
        private readonly IToastNotification _toastNotification;
        [BindProperty]
        public int shopId { get; set; }


        public ShopGallaryModel(DajeejContext context, UserManager<ApplicationUser> userManage, IWebHostEnvironment host, IToastNotification toastNotification)
        {
            _context = context;
            _userManager = userManage;
            _host = host;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                var shopObj = await _userManager.GetUserAsync(User);
                shopId = shopObj.EntityId;
                return Page();
            }
            catch (Exception)
            {
                return Redirect("../SomethingwentError");
            }

        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {

                var shopObj = await _userManager.GetUserAsync(User);
                shopId = shopObj.EntityId;

                if (HttpContext.Request.Form.Files.Count() > 0)
                {
                    var uniqeFileName = "";

                    for (int i = 0; i < HttpContext.Request.Form.Files.Count(); i++)
                    {

                        string uploadFolder = Path.Combine(_host.WebRootPath, "images/Shop");

                        uniqeFileName = Guid.NewGuid() + "_" + Response.HttpContext.Request.Form.Files[i].FileName;

                        string uploadedImagePath = Path.Combine(uploadFolder, uniqeFileName);

                        using (FileStream fileStream = new FileStream(uploadedImagePath, FileMode.Create))
                        {
                            Response.HttpContext.Request.Form.Files[i].CopyTo(fileStream);
                        }


                        ShopImage shopImg = new ShopImage()
                        {
                            ImageName = "images/Shop/" + uniqeFileName,
                            ShopId = shopId
                        };

                        _context.ShopImages.Add(shopImg);



                    }

                    _context.SaveChanges();


                }

            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("somthing went error");
            }


            return Redirect("/shop/Gallary/ShopGallary");
        }
    }
}
