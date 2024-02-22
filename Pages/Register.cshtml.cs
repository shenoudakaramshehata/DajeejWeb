using Dajeej.Data;
using Dajeej.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
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
using Microsoft.EntityFrameworkCore;

namespace Dajeej.Pages
{
    public class RegisterModel : PageModel
    {

        [BindProperty]
        public Shop shop { get; set; }
        private DajeejContext _context;
        private readonly IToastNotification _toastNotification;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostEnvironment;
        [BindProperty]
        public int planId { get; set; }
        public List<Country> countryList { get; set; }
        public List<EntityType> entityTypeList { get; set; }
        [BindProperty(SupportsGet = true)]
        public string url { get; set; }
        static string token = "rLtt6JWvbUHDDhsZnfpAhpYk4dxYDQkbcPTyGaKp2TYqQgG7FGZ5Th_WD53Oq8Ebz6A53njUoo1w3pjU1D4vs_ZMqFiz_j0urb_BH9Oq9VZoKFoJEDAbRZepGcQanImyYrry7Kt6MnMdgfG5jn4HngWoRdKduNNyP4kzcp3mRv7x00ahkm9LAK7ZRieg7k1PDAnBIOG3EyVSJ5kK4WLMvYr7sCwHbHcu4A5WwelxYK0GMJy37bNAarSJDFQsJ2ZvJjvMDmfWwDVFEVe_5tOomfVNt6bOg9mexbGjMrnHBnKnZR1vQbBtQieDlQepzTZMuQrSuKn-t5XZM7V6fCW7oP-uXGX-sMOajeX65JOf6XVpk29DP6ro8WTAflCDANC193yof8-f5_EYY-3hXhJj7RBXmizDpneEQDSaSz5sFk0sV5qPcARJ9zGG73vuGFyenjPPmtDtXtpx35A-BVcOSBYVIWe9kndG3nclfefjKEuZ3m4jL9Gg1h2JBvmXSMYiZtp9MR5I6pvbvylU_PP5xJFSjVTIz7IQSjcVGO41npnwIxRXNRxFOdIUHn0tjQ-7LwvEcTXyPsHXcMD8WtgBh-wxR8aKX7WPSsT1O8d8reb2aR7K3rkV3K82K_0OgawImEpwSvp9MNKynEAJQS6ZHe_J_l77652xwPNxMRTMASk1ZsJL";
        private readonly IConfiguration _configuration;
        public HttpClient httpClient { get; set; }
        public RegisterModel(
            DajeejContext context, IWebHostEnvironment hostEnvironment, IToastNotification toastNotification,
             UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db,
            IEmailSender emailSender
            , IConfiguration configuration
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
            _configuration = configuration;
        }
        public ActionResult OnGet()
        {
            
            try
            {
                url = $"{this.Request.Scheme}://{this.Request.Host}";
                countryList = _context.Countries.Where(e => e.IsActive == true).ToList();
                entityTypeList = _context.EntityTypes.ToList();
               
            }
            catch (Exception)
            {
                return RedirectToPage("SomethingwentError");
            }

            return Page();

        }
        public async Task<IActionResult> OnPost(int selectedValue, IFormFile Banner, IFormFile Logo,IFormFileCollection MorePhoto)
        {
            shop.RegisterDate = DateTime.Now;

            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Please ! Enter Required Data");
                return Page();

            }
            if (selectedValue ==0)
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
                shop.ShopImage = ShopImageList;

            }
            try
            {
                var user = new ApplicationUser { UserName = shop.Email, Email = shop.Email };
                var result = await _userManager.CreateAsync(user, shop.Password);
                Shop model = null;
                if (result.Succeeded)
                {

                    model = new Shop()
                    {
                        Address = shop.Address,
                        Email = shop.Email,
                        ShopNo = shop.ShopNo,
                        Password = shop.Password,
                        Mobile = shop.Mobile,
                        DeliveryCost = shop.DeliveryCost,
                        IsActive = false,
                        ShopTLAR = shop.ShopTLAR,
                        ShopTLEN = shop.ShopTLEN,
                        DescriptionTLAR = shop.DescriptionTLAR,
                        DescriptionTLEN = shop.DescriptionTLEN,
                        Instgram=shop.Instgram,
                        Tele = shop.Tele,
                        RegisterDate = DateTime.Now,
                        OrderIndex = 1,
                        CountryId = shop.CountryId,
                        Lat = shop.Lat,
                        Lng = shop.Lng,
                        Pic = shop.Pic,
                        Banner = shop.Banner,
                        EntityTypeId=shop.EntityTypeId,
                       ShopImage=shop.ShopImage
                        
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
                var planObj = _context.Plans.Find(selectedValue);
                var subscription = new Subscription()
                {
                    PlanId = selectedValue,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(planObj.Period),
                    ShopId = user.EntityId,
                    Active = false,
                    Price = planObj.Price

                };
                _context.Subscriptions.Add(subscription);
                _context.SaveChanges();
                double TotalCost = planObj.Price;

                bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
                var TestToken = _configuration["TestToken"];
                var LiveToken = _configuration["LiveToken"];
                if (Fattorahstatus) // fattorah live
                {
                    var sendPaymentRequest = new
                    {

                        CustomerName = model.ShopTLEN,
                        NotificationOption = "LNK",
                        InvoiceValue = TotalCost,
                        CallBackUrl = "http://dajeejapp.com/FattorahSuccess",
                        ErrorUrl = "http://dajeejapp.com/FattorahError",
                        UserDefinedField = subscription.SubscriptionId,
                        CustomerEmail = model.Email

                    };
                    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                    string url = "https://api.myfatoorah.com/v2/SendPayment";
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LiveToken);
                    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                    var responseMessage = httpClient.PostAsync(url, httpContent);
                    var res = await responseMessage.Result.Content.ReadAsStringAsync();
                    var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                    if (FattoraRes.IsSuccess == true)
                    {
                        Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                        var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                        return Redirect(InvoiceRes.InvoiceURL);

                    }
                    else
                    {
                        return RedirectToPage("SomethingwentError", new { Message = FattoraRes.Message });
                    }
                }
                else               // fattorah test
                {
                    var sendPaymentRequest = new
                    {

                        CustomerName = model.ShopTLEN,
                        NotificationOption = "LNK",
                        InvoiceValue = TotalCost,
                        CallBackUrl = "http://dajeejapp.com/FattorahSuccess",
                        ErrorUrl = "http://dajeejapp.com/FattorahError",
                        UserDefinedField = subscription.SubscriptionId,
                        CustomerEmail = model.Email

                    };
                    var sendPaymentRequestJSON = JsonConvert.SerializeObject(sendPaymentRequest);

                    string url = "https://apitest.myfatoorah.com/v2/SendPayment";
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestToken);
                    var httpContent = new StringContent(sendPaymentRequestJSON, Encoding.UTF8, "application/json");
                    var responseMessage = httpClient.PostAsync(url, httpContent);
                    var res = await responseMessage.Result.Content.ReadAsStringAsync();
                    var FattoraRes = JsonConvert.DeserializeObject<FattorhResult>(res);


                    if (FattoraRes.IsSuccess == true)
                    {
                        Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(res);
                        var InvoiceRes = jObject["Data"].ToObject<InvoiceData>();
                        return Redirect(InvoiceRes.InvoiceURL);

                    }
                    else
                    {
                        return RedirectToPage("SomethingwentError", new { Message = FattoraRes.Message });
                    }
                }


            }
            catch (Exception)
            {
                _toastNotification.AddErrorToastMessage("Somthing Went Error");
            }
            return Page();
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
