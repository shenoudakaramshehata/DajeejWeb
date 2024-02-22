using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dajeej.Data;
using Dajeej.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NToastNotify;
using System;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using MimeKit;
using Microsoft.EntityFrameworkCore;
namespace Dajeej.Areas.Admin.Pages.Shop
{
    public class EditShopModel : PageModel
    {
        [BindProperty]
        public Dajeej.Models.ShopVm shop { get; set; }
        private DajeejContext _context;
        private readonly IToastNotification _toastNotification;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        private IHostingEnvironment _env;
        [BindProperty]
        public int planId { get; set; }
        [BindProperty]
        public int shopId { get; set; }
        public List<Country> countryList { get; set; }
        public List<EntityType> entityTypeList { get; set; }
        public HttpClient httpClient { get; set; }
        public EditShopModel(
            DajeejContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification,
             UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db,
             IHostingEnvironment env

            )
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            httpClient = new HttpClient();
            _env = env;

        }
        public ActionResult OnGet(int id)
        {

            var shopObj = _context.Shops.Where(e => e.ShopId == id).Include(e => e.ShopImage).FirstOrDefault();
            var subObj = _context.Subscriptions.Where(e => e.ShopId == id).OrderByDescending(e => e.SubscriptionId).FirstOrDefault();
            shop = new ShopVm
            {
                Address = shopObj.Address,
                Email = shopObj.Email,
                ShopNo = shopObj.ShopNo,
                Password = shopObj.Password,
                Mobile = shopObj.Mobile,
                DeliveryCost = shopObj.DeliveryCost,
                ShopTLAR = shopObj.ShopTLAR,
                ShopTLEN = shopObj.ShopTLEN,
                DescriptionTLAR = shopObj.DescriptionTLAR,
                DescriptionTLEN = shopObj.DescriptionTLEN,
                Instgram = shopObj.Instgram,
                Tele = shopObj.Tele,
                OrderIndex = shopObj.OrderIndex,
                CountryId = shopObj.CountryId,
                Lat = shopObj.Lat,
                Lng = shopObj.Lng,
                Pic = shopObj.Pic,
                Banner = shopObj.Banner,
                EntityTypeId = shopObj.EntityTypeId,
                PlanId = subObj.PlanId.Value
               
                
            
            };
            shopId = id;
            return Page();


        }

        public async Task<IActionResult> OnPost(IFormFile Banner, IFormFile Logo, IFormFileCollection MorePhoto)
        {

            var shopObj = _context.Shops.Where(e => e.ShopId == shopId).Include(e => e.ShopImage).FirstOrDefault();

            var subObj = _context.Subscriptions.Where(e => e.ShopId == shopId).OrderByDescending(e => e.SubscriptionId).FirstOrDefault();


            var user = _userManager.Users.Where(e => e.EntityId == shopObj.ShopId).FirstOrDefault();

            if (Logo != null)
            {
                string folder = "Images/Shop/";
                shopObj.Pic = UploadImage(folder, Logo);
                user.Pic = shop.Pic;
            }
            else
            {
                shopObj.Pic = shop.Pic;
               
            }
          
            if (Banner != null)
            {
                string folder = "Images/Shop/";
                shopObj.Banner = UploadImage(folder, Banner);
            }
            else
            {
                shopObj.Banner = shop.Banner;
            }
            List<ShopImage> ShopImageList = new List<ShopImage>();
            if (MorePhoto.Count != 0)
            {
                foreach (var item in MorePhoto)
                {
                    var ShopImage = new ShopImage();
                    string folder = "Images/Shop/";
                    ShopImage.ImageName = UploadImage(folder, item);
                    ShopImage.ShopId = shopObj.ShopId;
                    ShopImageList.Add(ShopImage);
                }
                _context.ShopImages.AddRange(ShopImageList);

            }
            try
            {
                user.PhoneNumber = shop.Mobile;
                shopObj.Address = shop.Address;
                shopObj.Email = shopObj.Email;
                shopObj.ShopNo = shop.ShopNo;
                shopObj.Password = shopObj.Password;
                shopObj.Mobile = shop.Mobile;
                shopObj.DeliveryCost = shop.DeliveryCost;
                shopObj.IsActive = shopObj.IsActive;
                shopObj.ShopTLAR = shop.ShopTLAR;
                shopObj.ShopTLEN = shop.ShopTLEN;
                shopObj.DescriptionTLAR = shop.DescriptionTLAR;
                shopObj.DescriptionTLEN = shop.DescriptionTLEN;
                shopObj.Instgram = shop.Instgram;
                shopObj.Tele = shop.Tele;
                shopObj.RegisterDate = shopObj.RegisterDate;
                shopObj.OrderIndex = shop.OrderIndex;
                shopObj.CountryId = shop.CountryId;
                shopObj.Lat = shop.Lat;
                shopObj.Lng = shop.Lng;
                shopObj.EntityTypeId = shop.EntityTypeId;
                
                _context.Attach(shopObj).State = EntityState.Modified;
                var planObj = _context.Plans.Find(shop.PlanId);
                subObj.EndDate = subObj.StartDate.AddMonths(planObj.Period);
                subObj.PlanId = shop.PlanId;
                subObj.Price = planObj.Price;
                _context.Attach(subObj).State = EntityState.Modified;
                _context.SaveChanges();
                 _db.SaveChanges();
                _toastNotification.AddSuccessToastMessage("Shop Edited Successfully");
            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Somthing Went Error");

            }
            return Redirect("./index");
        }

        private string UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
    }
}
