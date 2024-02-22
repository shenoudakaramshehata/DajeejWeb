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
using NToastNotify;

namespace Dajeej.Areas.Admin.Pages.Items
{
    [Authorize(Roles = "Admin")]

    public class EditItemModel : PageModel
    {
        private readonly DajeejContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _host;
        private readonly IToastNotification _toastNotification;
        public EditItemModel(DajeejContext context, UserManager<ApplicationUser> userManage, IWebHostEnvironment host, IToastNotification toastNotification)
        {
            _context = context;
            _userManager = userManage;
            _host = host;
            _toastNotification = toastNotification;
        }

        [BindProperty]
        public Dajeej.Models.Item ItemImagesAndItemVm { get; set; }

        public void OnGet(int id)
        {
            ItemImagesAndItemVm  = _context.Items.FirstOrDefault(i => i.ItemId == id);

        }



        public async Task<IActionResult> OnPostAsync(Dajeej.Models.Item ItemImagesAndItemVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }



                if (_context.Items.FirstOrDefault(i => i.ItemId == ItemImagesAndItemVm.ItemId) != null)
                {
                    var item = _context.Items.FirstOrDefault(i => i.ItemId == ItemImagesAndItemVm.ItemId);
                    item.ItemTitleAr = ItemImagesAndItemVm.ItemTitleAr;
                    item.ItemTitleEn = ItemImagesAndItemVm.ItemTitleEn;
                    item.ItemPrice = ItemImagesAndItemVm.ItemPrice;
                    item.ItemDescriptionAr = ItemImagesAndItemVm.ItemDescriptionAr;
                    item.ItemDescriptionEn = ItemImagesAndItemVm.ItemDescriptionEn;
                    item.CategoryId = ItemImagesAndItemVm.CategoryId;
                    item.IsActive = ItemImagesAndItemVm.IsActive;
                    item.OrderIndex = ItemImagesAndItemVm.OrderIndex;
                    item.SubCategoryId = ItemImagesAndItemVm.SubCategoryId;
                    item.OutOfStock = ItemImagesAndItemVm.OutOfStock;
                    var itemCountry = _context.Shops.FirstOrDefault(s => s.ShopId == ItemImagesAndItemVm.ShopId).CountryId;
                    var UpdatedItem = _context.Items.Attach(item);
                    UpdatedItem.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();

                    if (HttpContext.Request.Form.Files.Count() > 0)
                    {
                        var uniqeFileName = "";

                        for (int i = 0; i < HttpContext.Request.Form.Files.Count(); i++)
                        {

                            string uploadFolder = Path.Combine(_host.WebRootPath, "Images/Item");

                            uniqeFileName = Guid.NewGuid() + "_" + Response.HttpContext.Request.Form.Files[i].FileName;

                            string uploadedImagePath = Path.Combine(uploadFolder, uniqeFileName);

                            using (FileStream fileStream = new FileStream(uploadedImagePath, FileMode.Create))
                            {
                                Response.HttpContext.Request.Form.Files[i].CopyTo(fileStream);
                            }

                            if (HttpContext.Request.Form.Files[i].Name == "MainImage")
                            {
                                item.ItemImage = uniqeFileName;
                            }
                            else
                            {
                                ItemImage proImg = new ItemImage()
                                {
                                    ImageName = uniqeFileName,
                                    ItemId = ItemImagesAndItemVm.ItemId
                                };

                                _context.ItemImages.Add(proImg);

                            }

                        }

                        _context.SaveChanges();
                    }

                }
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("somthing went error");
            }



            return Redirect("/Admin/Shop/ShopDetails?id=" + ItemImagesAndItemVm.ShopId);
        }
    }
}
