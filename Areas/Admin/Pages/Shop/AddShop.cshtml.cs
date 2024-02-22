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
    public class AddShopModel : PageModel
    {
        [BindProperty]
        public Dajeej.Models.ShopVm shop { get; set; }
        private DajeejContext _context;
        private readonly IToastNotification _toastNotification;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostEnvironment;
        private IHostingEnvironment _env;
        [BindProperty]
        public int planId { get; set; }
        public List<Country> countryList { get; set; }
        public List<EntityType> entityTypeList { get; set; }
        [BindProperty(SupportsGet = true)]
        public string url { get; set; }
        
        public HttpClient httpClient { get; set; }
        public AddShopModel(
            DajeejContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification,
             UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db,
             IHostingEnvironment env,
            IEmailSender emailSender
            
            )
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _toastNotification = toastNotification;
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _emailSender = emailSender;
            httpClient = new HttpClient();
            _env = env;

        }
        public ActionResult OnGet()
        {


            return Page();


        }

        public async Task<IActionResult> OnPost(IFormFile Banner, IFormFile Logo, IFormFileCollection MorePhoto)
        {
          

            
            if (shop.PlanId == 0)
            {
                _toastNotification.AddErrorToastMessage("You Must Select Plan !");
                return Page();
            }
            var userExist = _db.ApplicationUsers.FirstOrDefault(s => s.Email == shop.Email);
            if (userExist != null)
            {
                _toastNotification.AddErrorToastMessage("User Aleady Exist !");
                return Page();
            }

            if (Logo == null)
            {
                _toastNotification.AddErrorToastMessage("Please ! Enter Logo");
                return Page();
            }
            if (Banner == null)
            {
                _toastNotification.AddErrorToastMessage("Please ! Enter Logo");
                return Page();
            }
            if (Logo != null)
            {
                string folder = "Images/Shop/";
                shop.Pic = UploadImage(folder, Logo);
            }
            if (Banner != null)
            {
                string folder = "Images/Shop/";
                shop.Banner = UploadImage(folder, Banner);
            }
            List<ShopImage> ShopImageList = new List<ShopImage>();
            if (MorePhoto.Count != 0)
            {
                foreach (var item in MorePhoto)
                {
                    var ShopImage = new ShopImage();
                    string folder = "Images/Shop/";
                    ShopImage.ImageName = UploadImage(folder, item);
                    ShopImageList.Add(ShopImage);

                }
                

            }
            try
            {
                var user = new ApplicationUser { UserName = shop.Email, Email = shop.Email };
                var result = await _userManager.CreateAsync(user, shop.Password);
                Dajeej.Models.Shop model = null;
                if (result.Succeeded)
                {

                    model = new Dajeej.Models.Shop()
                    {
                        Address = shop.Address,
                        Email = shop.Email,
                        ShopNo = shop.ShopNo,
                        Password = shop.Password,
                        Mobile = shop.Mobile,
                        DeliveryCost = shop.DeliveryCost,
                        IsActive = true,
                        ShopTLAR = shop.ShopTLAR,
                        ShopTLEN = shop.ShopTLEN,
                        DescriptionTLAR = shop.DescriptionTLAR,
                        DescriptionTLEN = shop.DescriptionTLEN,
                        Instgram = shop.Instgram,
                        Tele = shop.Tele,
                        RegisterDate = DateTime.Now,
                        OrderIndex = shop.OrderIndex,
                        CountryId = shop.CountryId,
                        Lat = shop.Lat,
                        Lng = shop.Lng,
                        Pic = shop.Pic,
                        Banner = shop.Banner,
                        EntityTypeId = shop.EntityTypeId,
                        ShopImage =ShopImageList

                     };
                    _context.Shops.Add(model);


                }
                await _userManager.AddToRoleAsync(user, "Shop");
                await _context.SaveChangesAsync();
                user.EntityId = model.ShopId;
                user.Pic = model.Pic;
                user.PhoneNumber = model.Mobile;
                _db.Attach(user).State = EntityState.Modified;
                _db.SaveChanges();
                var planObj = _context.Plans.Find(shop.PlanId);
                var subscription = new Subscription()
                {
                    PlanId = shop.PlanId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(planObj.Period),
                    ShopId = user.EntityId,
                    Active = true,
                    Price = planObj.Price

                };
                _context.Subscriptions.Add(subscription);
                _context.SaveChanges();
                double TotalCost = planObj.Price;
                var webRoot = _env.WebRootPath;

                var pathToFile = _env.WebRootPath
                       + Path.DirectorySeparatorChar.ToString()
                       + "Templates"
                       + Path.DirectorySeparatorChar.ToString()
                       + "EmailTemplate"
                       + Path.DirectorySeparatorChar.ToString()
                       + "Email.html";
                var builder = new BodyBuilder();
                using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                {

                    builder.HtmlBody = SourceReader.ReadToEnd();

                }
                string messageBody = string.Format(builder.HtmlBody,
                   subscription.StartDate.ToShortDateString(),
                   subscription.EndDate.ToShortDateString(),
                   TotalCost,
                   shop.ShopTLAR,
                   planObj.ArabicTitle

                   );
                await _emailSender.SendEmailAsync(shop.Email, "Shop Subscription", messageBody);

                _toastNotification.AddSuccessToastMessage("Shop Added Successfully");


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
