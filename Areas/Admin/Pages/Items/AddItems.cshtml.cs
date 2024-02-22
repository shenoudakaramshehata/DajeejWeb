using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dajeej.Data;
using Dajeej.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace Dajeej.Areas.Admin.Pages.Items
{
    [Authorize(Roles = "Admin")]

    public class ItemsModel : PageModel
    {
       
        private readonly DajeejContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _host;
        private readonly IToastNotification _toastNotification;

        [BindProperty]
        public int Id { get; set; }
        [BindProperty]
        public Dajeej.Models.Item Item { get; set; }

        public ItemsModel(DajeejContext context, UserManager<ApplicationUser> userManage, IWebHostEnvironment host, IToastNotification toastNotification)
        {
            _context = context;
            _userManager = userManage;
            _host = host;
            _toastNotification = toastNotification;
        }
        public void OnGet(int id)
        {
            Id = id;
        }
        

        public async Task<IActionResult> OnPostAsync(Dajeej.Models.Item Item)
        {
            try
            {
                var lastSubscription = _context.Subscriptions.Include(q => q.Plan).Where(s => s.ShopId ==Id).OrderBy(i => i.SubscriptionId).LastOrDefault();
            var numberOfAvailableItems = lastSubscription.Plan.NoOfItems;

            var numberOfCurrentItems = _context.Items.Where(s => s.ShopId == Id).Count();
            

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (numberOfCurrentItems >= numberOfAvailableItems)
            {
               _toastNotification.AddErrorToastMessage("Exceed ,you have exceeded the number of available items");
                   
                return Page();
            }

            Item.ShopId = Id;

            var itemCountry = _context.Shops.FirstOrDefault(s => s.ShopId == Id).CountryId;

            _context.Items.Add(Item);
            await _context.SaveChangesAsync();

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
                        Item.ItemImage = uniqeFileName;
                        _context.Attach(Item).State = EntityState.Modified;

                    }
                    else
                    {
                        ItemImage proImg = new ItemImage()
                        {
                            ImageName = uniqeFileName,
                            ItemId = Item.ItemId
                        };

                        _context.ItemImages.Add(proImg);

                    }

                }
                    _context.SaveChanges();
                    

                    
            }


            var collectionsCheckBox = Request.Form["Collection"];

            foreach (var item in collectionsCheckBox)
            {
                var collectionId = int.Parse(item);

                CollectionItem col = new CollectionItem()
                {

                    CollectionId = collectionId,
                    ItemId = Item.ItemId

                };

                _context.CollectionItems.Add(col);

            }

            _context.SaveChanges();
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Something Went Error");
            }
            _toastNotification.AddSuccessToastMessage("Item Addded Successfully");
            return Redirect("/Admin/Shop/ShopDetails?id="+Id);
        }

    }
}
