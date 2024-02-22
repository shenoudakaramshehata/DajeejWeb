using Dajeej.Data;
using Dajeej.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Dajeej.ViewModels;
using MimeKit;

namespace Dajeej.Controllers
{

    [Route("api/[Controller]/[action]")]
    public class MobileController : Controller
    {
        #region Properites
        private readonly DajeejContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        public HttpClient httpClient { get; set; }
        #endregion
        public MobileController(DajeejContext context, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
            ApplicationDbContext db, IWebHostEnvironment hostEnvironment, IConfiguration configuration)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
            _hostEnvironment = hostEnvironment;
            httpClient = new HttpClient();
            _emailSender = emailSender;
            _configuration = configuration;

        }
        #region User
        #region Login
        [HttpGet]
        public async Task<ActionResult<ApplicationUser>> Login([FromQuery] string Email, [FromQuery] string Password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(Email);

                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, Password, true);
                    if (result.Succeeded)
                    {
                        return Ok(new { status = true, Message = "User Login successfully!", user });

                    }
                }

                return Ok(new { status = false, message = "User Not Found" });
            }
            catch (Exception)
            {

                return Ok(new { status = false, message = "Something went wrong" });

            }


        }
        #endregion
        #region Register
        [HttpPost]
        public async Task<IActionResult> Register(CustomerVM customerVM, IFormFile customerImage)
        {
           
            if (validattRegisterModel(customerVM))
            {
                try
                {
                    var userExists = await _userManager.FindByEmailAsync(customerVM.Email);

                    if (userExists != null)
                    {
                        return Ok(new { status = false, message = "Email Aleady Exists" });
                    }
                    var user = new ApplicationUser { UserName = customerVM.Email, Email = customerVM.Email };
                    var result = await _userManager.CreateAsync(user, customerVM.Password);

                    if (!result.Succeeded)
                    {
                        return Ok(new { status = false, message = "User creation failed! Please check user details and try again." });
                    }
                    Customer customer = new Customer()
                    {
                        Email = customerVM.Email,
                        NameAr = customerVM.NameAr,
                        NameEn = customerVM.NameEn,
                        Mobile = customerVM.Mobile,
                        //Address = customerVM.Address,
                        //Lat = customerVM.Lat,
                        //Lng = customerVM.Lng,
                        RegisterDate = DateTime.Now,
                    };
                    if (customerImage != null)
                    {
                        string folder = "Images/CustomerImages/";
                        customer.Pic = UploadImage(folder, customerImage);
                    }
                    _context.Customers.Add(customer);
                    _context.SaveChanges();
                    if (customerImage != null)
                    {
                        user.Pic = customer.Pic;
                    }
                    user.EntityId = customer.CustomerId;
                    _db.SaveChanges();
                    return Ok(new { Status = "Success", customerObj = customer, Message = "User created successfully!" });
                }
                catch (Exception ex)
                {
                    return Ok(new { status = false, message = ex.Message });

                }

            }
            return Ok(new { Status = false, message = "Please! Enter All Required Data" });
        }
        #endregion
        #region Change Password
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordVM changePasswordVM)
        {
            try
            {
                if (validateChangePasswordModel(changePasswordVM))
                {
                    var user = await _userManager.FindByEmailAsync(changePasswordVM.Email);
                    if (user == null)
                    {
                        return Ok(new { Status = false, Message = "User Not Found" });

                    }
                    var Result = await _userManager.ChangePasswordAsync(user, changePasswordVM.CurrentPassword, changePasswordVM.NewPassword);
                    if (!Result.Succeeded)
                    {
                        return Ok(new { Status = false, message = "Can Not Change Password Please! Try Again" });


                    }

                    return Ok(new { Status = true, Message = "Password has Changed" });

                }
            }
            catch (Exception ex)
            {

                return Ok(new { Status = false, message = ex.Message });

            }
            return Ok(new { status = false, message = "Please! Enter All Required Data" });

        }
        #endregion
        #region ForgetPassword
        [HttpPost]
        public async Task<IActionResult> ForgetPasswordAsync(string Email)
        {
            try
            {
                if (Email != null)
                {
                    var user = await _userManager.FindByEmailAsync(Email);
                    if (user == null)
                    {

                        return Ok(new { status = false, message = "User isn't Exist" });

                    }

                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var webRoot = _hostEnvironment.WebRootPath;

                    var pathToFile = _hostEnvironment.WebRootPath
                           + Path.DirectorySeparatorChar.ToString()
                           + "Templates"
                           + Path.DirectorySeparatorChar.ToString()
                           + "EmailTemplate"
                           + Path.DirectorySeparatorChar.ToString()
                           + "ResetPassword.html";
                    var builder = new BodyBuilder();
                    using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                    {

                        builder.HtmlBody = SourceReader.ReadToEnd();

                    }
                    string messageBody = string.Format(builder.HtmlBody,
                     user.UserName,
                     code,
                       string.Format("{0:dddd, d MMMM yyyy}", DateTime.Now)
                       );
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Reset Password",
                       messageBody);

                    return Ok(new { status = true, message = "Please check your email to reset your password." });

                }
                else
                {
                    return Ok(new { status = false, message = "Must send Email" });
                }
            }
            catch (Exception ex)
            {

                return Ok(new { status = false, message = ex.Message });

            }

        }
        #endregion
        #region Edit Profile
        [HttpPut]
        public IActionResult EditProfile(EditCustomerProfileVM editCustomerProfileVM, IFormFile customerImage)
        {
            if (validateEditCustomerProfileModel(editCustomerProfileVM))
            {
                try
                {
                    var customer = _context.Customers.FirstOrDefault(e => e.CustomerId == editCustomerProfileVM.CustomerId);
                    if (customer == null)
                    {
                        return Ok(new { Status = false, message = "User Not Exist" });
                    }

                    if (customerImage != null)
                    {
                        string folder = "Images/CustomerImages/";
                        customer.Pic = UploadImage(folder, customerImage);
                        _db.ApplicationUsers.FirstOrDefault(u => u.EntityId == editCustomerProfileVM.CustomerId).Pic = customer.Pic;
                        _db.SaveChanges();
                    }
                    customer.Address = editCustomerProfileVM.Address;
                    customer.Mobile = editCustomerProfileVM.Mobile;
                    customer.NameAr = editCustomerProfileVM.NameAr;
                    customer.NameEn = editCustomerProfileVM.NameEn;
                    _context.SaveChanges();
                    return Ok(new { Status = true, message = "Updated Successfully", NewCustomer = customer });
                }
                catch (Exception ex)
                {
                    return Ok(new { Status = false, message = ex.Message });

                }
            }
            return Ok(new { Status = false, message = "Please! Enter All Required Data" });



        }
        #endregion
        #region AddMessage
        [HttpPost]
        public IActionResult AddMessage([FromBody] MessageVM messageVM)
        {
            try
            {
                var model = new ContactUs()
                {
                    Email = messageVM.Email,
                    FullName = messageVM.FullName,
                    Mobile = messageVM.Mobile,
                    Msg = messageVM.Msg,
                    TransDate = DateTime.Now

                };
                _context.ContactUs.Add(model);
                _context.SaveChanges();
                return Ok(new { status = true, Message = "Message Sent Successfully" });

            }
            catch (Exception)
            {

                return Ok(new { status = false, message = "Something went wrong" });

            }

        }
        #endregion
        #region AddCustomerAddress
        [HttpPost]
        public IActionResult AddCustomerAddress([FromBody] CustomerAddressVM customerAddressVM)
        {
            try
            {
                var customerObj = _context.Customers.Where(e => e.CustomerId == customerAddressVM.CustomerId).FirstOrDefault();
                if (customerObj == null)
                {
                    return Ok(new { Status = false, message = "Customer Object Not Found" });

                }
                var addressTypeObj = _context.AddressTypes.Where(e => e.AddressTypeId == customerAddressVM.AddressTypeId).FirstOrDefault();
                if (addressTypeObj == null)
                {
                    return Ok(new { Status = false, message = "Address Type Object Not Found" });

                }
                var customerAddress = new CustomerAddress()
                {
                    AddressTypeId = customerAddressVM.AddressTypeId,
                    CustomerId = customerAddressVM.CustomerId,
                    AddressNickname = customerAddressVM.AddressNickname,
                    Area = customerAddressVM.Area,
                    Avenue = customerAddressVM.Avenue,
                    AdditionalDirection = customerAddressVM.AdditionalDirection,
                    Block = customerAddressVM.Block,
                    Building = customerAddressVM.Building,
                    Floor = customerAddressVM.Floor,
                    Lat = customerAddressVM.Lat,
                    Lng = customerAddressVM.Lng,
                    Office = customerAddressVM.Office,
                    Street = customerAddressVM.Street,

                };
                _context.CustomerAddresses.Add(customerAddress);
                _context.SaveChanges();
                return Ok(new { status = true, Message = "Address Added Successfully", CustomerAddress = customerAddress });

            }
            catch (Exception ex)
            {

                return Ok(new { status = false, message = ex.Message });

            }

        }
        #endregion
        #region DeleteCustomerAddress
        [HttpDelete]
        public IActionResult DeleteCustomerAddress(int CustomerAddressId)
        {
            try
            {
                var customerAddObj = _context.CustomerAddresses.Where(e => e.CustomerAddressId == CustomerAddressId).FirstOrDefault();
                if (customerAddObj == null)
                {
                    return Ok(new { Status = false, message = "Customer Address Object Not Found" });

                }

                _context.CustomerAddresses.Remove(customerAddObj);
                _context.SaveChanges();
                return Ok(new { status = true, Message = "Address Deleted Successfully" });

            }
            catch (Exception ex)
            {

                return Ok(new { status = false, message = ex.Message });

            }

        }
        #endregion
        #region getAllAddressType
        [HttpGet]
        public IActionResult GettAllAddressType()
        {
            try
            {
                var adddressTypes = _context.AddressTypes.ToList();
                return Ok(new { status = true, AdddressTypes = adddressTypes });

            }
            catch (Exception ex)
            {

                return Ok(new { status = false, message = ex.Message });

            }

        }
        #endregion
        #region EditCustomerAddress
        [HttpPut]
        public IActionResult EditCustomerAddress([FromBody] EditCustomerAddressVM editCustomerAddressVM)
        {
            try
            {
                //var customerAddressObj = _context.CustomerAddresses.Where(e => e.CustomerAddressId == editCustomerAddressVM.CustomerAddressId).FirstOrDefault();
                //if (customerAddressObj == null)
                //{
                //    return Ok(new { Status = false, message = "Customer Address Object Not Found" });

                //}
                var customerObj = _context.Customers.Where(e => e.CustomerId == editCustomerAddressVM.CustomerId).FirstOrDefault();
                if (customerObj == null)
                {
                    return Ok(new { Status = false, message = "Customer Object Not Found" });

                }
                var addressTypeObj = _context.AddressTypes.Where(e => e.AddressTypeId == editCustomerAddressVM.AddressTypeId).FirstOrDefault();
                if (addressTypeObj == null)
                {
                    return Ok(new { Status = false, message = "Address Type Object Not Found" });

                }
                CustomerAddress customerAddress = new CustomerAddress()
                {
                    CustomerAddressId = editCustomerAddressVM.CustomerAddressId,
                    AddressTypeId = editCustomerAddressVM.AddressTypeId,
                    CustomerId = editCustomerAddressVM.CustomerId,
                    AddressNickname = editCustomerAddressVM.AddressNickname,
                    Area = editCustomerAddressVM.Area,
                    Avenue = editCustomerAddressVM.Avenue,
                    AdditionalDirection = editCustomerAddressVM.AdditionalDirection,
                    Block = editCustomerAddressVM.Block,
                    Building = editCustomerAddressVM.Building,
                    Floor = editCustomerAddressVM.Floor,
                    Lat = editCustomerAddressVM.Lat,
                    Lng = editCustomerAddressVM.Lng,
                    Office = editCustomerAddressVM.Office,
                    Street = editCustomerAddressVM.Street,

                };
                _context.Attach(customerAddress).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok(new { status = true, Message = "Address Edited Successfully", CustomerAddress = customerAddress });




            }
            catch (Exception ex)
            {

                return Ok(new { status = false, message = ex.Message });

            }

        }
        #endregion

        #region GetCustomerAddressById
        [HttpGet]
        public IActionResult GetCustomerAddressById(int customerAddressId)
        {
            try
            {
                var customerAddress = _context.CustomerAddresses.Where(e => e.CustomerAddressId == customerAddressId).FirstOrDefault();
                if (customerAddress == null)
                {
                    return Ok(new { Status = false, message = "Customer Address Object Not Found" });

                }

                return Ok(new { Status = true, CustomerAddress = customerAddress });

            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }


        }
        #endregion
        #region CustomersAddress By CustomerId
        [HttpGet]
        public IActionResult GetAllCustomersAddressByCustomerId(int customerId)
        {
            try
            {
                var customerObj = _context.Customers.Where(e => e.CustomerId == customerId).FirstOrDefault();
                if (customerObj == null)
                {
                    return Ok(new { Status = false, message = "Customer Object Not Found" });

                }
                var customerAddressList = _context.CustomerAddresses.Where(e => e.CustomerId == customerId).ToList();
                return Ok(new { Status = true, CustomerAddressList = customerAddressList });

            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }


        }
        #endregion
        #endregion
        #region Public Functions
        [ApiExplorerSettings(IgnoreApi = true)]
        private string UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);

            file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return folderPath;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private bool UpdateAddress(CustomerAddress customerAddress)
        {
            try
            {
                _context.Attach(customerAddress).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }


        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public bool HasCurrenctlySubscription(int Shopid)
        {
            var checkHasCurrenctlySub = _context.Subscriptions.Any(c => c.ShopId == Shopid && (c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now) && c.Active == true);
            if (checkHasCurrenctlySub)
            {
                return true;
            }

            return false;

        }
        #endregion
        #region Validation Methods
        [ApiExplorerSettings(IgnoreApi = true)]
        private bool validattRegisterModel(CustomerVM customerVM)
        {
            if (customerVM.Email != null && customerVM.NameEn != null && customerVM.Password != null && customerVM.NameAr != null && customerVM.Mobile != null)
            {
                return true;
            }
            return false;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private bool validateEditCustomerProfileModel(EditCustomerProfileVM editCustomerProfileVM)
        {
            if (editCustomerProfileVM.CustomerId != 0 && editCustomerProfileVM.Address != null && editCustomerProfileVM.Mobile != null && editCustomerProfileVM.NameAr != null && editCustomerProfileVM.NameEn != null)
            {
                return true;
            }
            return false;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        private bool validateChangePasswordModel(ChangePasswordVM changePasswordVM)
        {
            if (changePasswordVM.Email != null && changePasswordVM.CurrentPassword != null && changePasswordVM.NewPassword != null)
            {
                return true;
            }
            return false;
        }
        #endregion
        #region GetByIdFunctions
        #region Customers
        [HttpGet]
        public IActionResult GetCustomersById(int customerId)
        {
            try
            {
                var customer = _context.Customers.Where(c => c.CustomerId == customerId).FirstOrDefault();
                if (customer == null)
                {
                    return Ok(new
                    {
                        Status = false,
                        message = "Customer Not Found "
                    });
                }
                return Ok(new { Status = true, customer = customer });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });

            }

        }
        #endregion
        #region User
        [HttpGet]
        public IActionResult GetUserById(int userId)
        {
            try
            {
                var user = _db.ApplicationUsers.Where(e => e.EntityId == userId).FirstOrDefault();
                if (user == null)
                {
                    return Ok(new
                    {
                        Status = false,
                        message = "User Not Found "
                    });
                }



                return Ok(new { Status = true, user = user });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });

            }

        }
        #endregion
        #region Page Content
        [HttpGet]
        public IActionResult GetPagesContentById([FromQuery] int PageContentId)
        {

            try
            {

                var pageContent = _context.PageContents.FirstOrDefault(c => c.PageContentId == PageContentId);
                if (pageContent == null)
                {
                    return Ok(new { status = false, message = "Object Not Found" });

                }
                return Ok(new { status = true, pageContent = pageContent });

            }
            catch (Exception)
            {

                return Ok(new { status = false, message = "Something went wrong" });

            }
        }
        #endregion
        #region Shops

        [HttpGet]
        public IActionResult GetShopById([FromQuery] int shopId)
        {

            try
            {

                var shop = _context.Shops.Include(c => c.ShopImage).Where(c => c.ShopId == shopId).Select(i => new
                {
                    ShopId = i.ShopId,
                    ShopTLAR = i.ShopTLAR,
                    ShopTLEN = i.ShopTLEN,
                    Address = i.Address,
                    Tele = i.Tele,
                    ShopNo = i.ShopNo,
                    Banner = i.Banner,
                    Pic = i.Pic,
                    Lat = i.Lat,
                    Lng = i.Lng,
                    Instgram = i.Instgram,
                    DescriptionTLEN = i.DescriptionTLEN,
                    DescriptionTLAR = i.DescriptionTLAR,
                    DeliveryCost = i.DeliveryCost,
                    ShopImage = i.ShopImage,

                }).FirstOrDefault();

                if (shop == null)
                {
                    return Ok(new { status = false, message = "Shop Object Not Found" });

                }
                return Ok(new { status = true, Shop = shop });

            }
            catch (Exception)
            {

                return Ok(new { status = false, message = "Something went wrong" });

            }
        }
        #endregion
        #region Items
        #region ItemsById
        [HttpGet]
        public IActionResult GetItemById(int itemId, int customerId)
        {
            try
            {
                var itemObj = _context.Items.Where(e => e.ItemId == itemId).FirstOrDefault();
                if (itemObj == null)
                {
                    return Ok(new { Status = false, message = "Item Object Not Found" });

                }
                var item = _context.Items.Include(s => s.Shop).Include(c => c.ItemImages).Where(c => c.ItemId == itemId)

                .Select(c => new
                {
                    CategoryId = c.CategoryId,
                    ItemId = c.ItemId,
                    Shop = c.Shop,
                    ItemTitleAr = c.ItemTitleAr,
                    ItemTitleEn = c.ItemTitleEn,
                    ItemPrice = c.ItemPrice,
                    ItemDescriptionAr = c.ItemDescriptionAr,
                    ItemDescriptionEn = c.ItemDescriptionEn,
                    ItemImage = "Images/Item/" + c.ItemImage,
                    OutOfStock = c.OutOfStock,
                    ItemImages = c.ItemImages,
                    SubCategoryId = c.SubCategoryId,
                    IsFavourite = _context.Favourites.Any(o => o.ItemId == c.ItemId && o.CustomerId == customerId),

                }).FirstOrDefault();

                return Ok(new { Status = true, item = item });

            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }


        }
        #endregion
        #region ItemsByCategoryId
        [HttpGet]
        public IActionResult GetAllItemsByCategoryId(int categoryid, int customerId)
        {
            try
            {
                var categoryObj = _context.Categories.Where(e => e.CategoryId == categoryid).FirstOrDefault();
                if (categoryObj == null)
                {
                    return Ok(new { Status = false, message = "Category Object Not Found" });

                }


                var itemsList = _context.Items.Include(s => s.Shop).Include(c => c.ItemImages).Where(c => c.CategoryId == categoryid && c.IsActive == true && c.Shop.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault().Active && c.Shop.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault().EndDate >= DateTime.Now)
                .OrderBy(c => c.OrderIndex)
                .Select(c => new
                {
                    CategoryId = c.CategoryId,
                    ItemId = c.ItemId,
                    Shop = c.Shop,
                    ItemTitleAr = c.ItemTitleAr,
                    ItemTitleEn = c.ItemTitleEn,
                    ItemPrice = c.ItemPrice,
                    ItemDescriptionAr = c.ItemDescriptionAr,
                    ItemDescriptionEn = c.ItemDescriptionEn,
                    ItemImage = "Images/Item/" + c.ItemImage,
                    OutOfStock = c.OutOfStock,
                    ItemImages = c.ItemImages,
                    SubCategoryId = c.SubCategoryId,
                    IsFavourite = _context.Favourites.Any(o => o.ItemId == c.ItemId && o.CustomerId == customerId),


                });

                return Ok(new { Status = true, itemsList = itemsList });

            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }


        }
        #endregion
        #region ItemsBySubCategoryId
        [HttpGet]
        public IActionResult GetAllItemsBySubCategoryId(int subCategoryid, int customerId)
        {
            try
            {
                var subCategoryObj = _context.SubCategories.Where(e => e.SubCategoryId == subCategoryid).FirstOrDefault();
                if (subCategoryObj == null)
                {
                    return Ok(new { Status = false, message = "SubCategory Object Not Found" });

                }


                var itemsList = _context.Items.Include(s => s.Shop).Include(c => c.ItemImages).Where(c => c.SubCategoryId == subCategoryid && c.IsActive == true && c.Shop.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault().Active && c.Shop.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault().EndDate >= DateTime.Now)
                .OrderBy(c => c.OrderIndex)
                .Select(c => new
                {
                    CategoryId = c.CategoryId,
                    ItemId = c.ItemId,
                    Shop = c.Shop,
                    ItemTitleAr = c.ItemTitleAr,
                    ItemTitleEn = c.ItemTitleEn,
                    ItemPrice = c.ItemPrice,
                    ItemDescriptionAr = c.ItemDescriptionAr,
                    ItemDescriptionEn = c.ItemDescriptionEn,
                    ItemImage = "Images/Item/" + c.ItemImage,
                    OutOfStock = c.OutOfStock,
                    ItemImages = c.ItemImages,
                    SubCategoryId = c.SubCategoryId,
                    IsFavourite = _context.Favourites.Any(o => o.ItemId == c.ItemId && o.CustomerId == customerId),

                });

                return Ok(new { Status = true, itemsList = itemsList });

            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }


        }
        #endregion
        #region Items By ShopId
        [HttpGet]
        public ActionResult GetItemsForEachShop(int shopid, int customerId)
        {
            try
            {
                var shopObj = _context.Shops.Where(e => e.ShopId == shopid).FirstOrDefault();
                if (shopObj == null)
                {
                    return Ok(new { Status = false, message = "Shop Object Not Found" });

                }
                if (HasCurrenctlySubscription(shopid))
                {
                    var itemsList = _context.Items.Include(s => s.Shop).Include(c => c.ItemImages).Where(c => c.ShopId == shopid && c.IsActive == true)
                    .OrderBy(c => c.OrderIndex)
                    .Select(c => new
                    {
                        CategoryId = c.CategoryId,
                        ItemId = c.ItemId,
                        Shop = c.Shop,
                        ItemTitleAr = c.ItemTitleAr,
                        ItemTitleEn = c.ItemTitleEn,
                        ItemPrice = c.ItemPrice,
                        ItemDescriptionAr = c.ItemDescriptionAr,
                        ItemDescriptionEn = c.ItemDescriptionEn,
                        ItemImage = "Images/Item/" + c.ItemImage,
                        OutOfStock = c.OutOfStock,
                        ItemImages = c.ItemImages,
                        SubCategoryId = c.SubCategoryId,
                        IsFavourite = _context.Favourites.Any(o => o.ItemId == c.ItemId && o.CustomerId == customerId),

                    });
                    return Ok(new { Status = true, itemsList = itemsList });
                }
                else
                {
                    return Ok(new { Status = false, message = "Shop Not Subscribed" });

                }


            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }



        }
        #endregion
        #region GetLatestItemsByShopId
        [HttpGet]
        public ActionResult GetLatestItems(int customerId)
        {
            try
            {


                var itemsList = _context.Items.Include(s => s.Shop).Include(c => c.ItemImages).Where(c => c.IsActive == true && c.Shop.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault().Active && c.Shop.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault().EndDate >= DateTime.Now).OrderByDescending(c => c.ItemId).Take(8)
                .Select(c => new
                {
                    CategoryId = c.CategoryId,
                    ItemId = c.ItemId,
                    Shop = c.Shop,
                    ItemTitleAr = c.ItemTitleAr,
                    ItemTitleEn = c.ItemTitleEn,
                    ItemPrice = c.ItemPrice,
                    ItemDescriptionAr = c.ItemDescriptionAr,
                    ItemDescriptionEn = c.ItemDescriptionEn,
                    ItemImage = "Images/Item/" + c.ItemImage,
                    OutOfStock = c.OutOfStock,
                    ItemImages = c.ItemImages,
                    SubCategoryId = c.SubCategoryId,
                    IsFavourite = _context.Favourites.Any(o => o.ItemId == c.ItemId && o.CustomerId == customerId),

                });

                return Ok(new { Status = true, itemsList = itemsList });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }



        }
        #endregion

        #region Related Items 
        [HttpGet]
        public ActionResult GetRelatedItems(int itemId, int customerId)
        {
            try
            {
                var itemObj = _context.Items.Where(e => e.ItemId == itemId).FirstOrDefault();
                if (itemObj == null)
                {
                    return Ok(new { Status = false, message = "Item Object Not Found" });

                }
                var itemsList = _context.Items.Include(s => s.Shop).Include(c => c.ItemImages).Where(c => c.CategoryId == itemObj.CategoryId && c.ShopId == itemObj.ShopId && c.SubCategoryId == itemObj.SubCategoryId && c.ItemId != itemObj.ItemId && c.IsActive == true && c.Shop.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault().Active && c.Shop.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault().EndDate >= DateTime.Now)
                .OrderBy(c => c.OrderIndex)
                .Select(c => new
                {
                    CategoryId = c.CategoryId,
                    ItemId = c.ItemId,
                    Shop = c.Shop,
                    ItemTitleAr = c.ItemTitleAr,
                    ItemTitleEn = c.ItemTitleEn,
                    ItemPrice = c.ItemPrice,
                    ItemDescriptionAr = c.ItemDescriptionAr,
                    ItemDescriptionEn = c.ItemDescriptionEn,
                    ItemImage = "Images/Item/" + c.ItemImage,
                    OutOfStock = c.OutOfStock,
                    ItemImages = c.ItemImages,
                    SubCategoryId = c.SubCategoryId,
                    IsFavourite = _context.Favourites.Any(o => o.ItemId == c.ItemId && o.CustomerId == customerId),



                });
                return Ok(new { Status = true, itemsList = itemsList });


            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }



        }
        #endregion
        #region Search Item
        [HttpGet]
        public IActionResult SearchForItem(string searchWord)
        {
            try
            {

                if (searchWord == null || searchWord == "")
                {
                    return Ok(new { Status = false, message = "Enter At Least One Char" });

                }
                var itemsList = _context.Items.Include(e => e.Shop).Where(i => i.ItemTitleAr.Contains(searchWord) || i.ItemTitleEn.Contains(searchWord) || i.ItemDescriptionAr.Contains(searchWord) || i.ItemDescriptionEn.Contains(searchWord) || i.Shop.ShopTLAR.Contains(searchWord) || i.Shop.ShopTLEN.Contains(searchWord))
                    .Select(c => new
                    {
                        CategoryId = c.CategoryId,
                        ItemId = c.ItemId,
                        Shop = c.Shop,
                        ItemTitleAr = c.ItemTitleAr,
                        ItemTitleEn = c.ItemTitleEn,
                        ItemPrice = c.ItemPrice,
                        ItemDescriptionAr = c.ItemDescriptionAr,
                        ItemDescriptionEn = c.ItemDescriptionEn,
                        ItemImage = "Images/Item/" + c.ItemImage,
                        OutOfStock = c.OutOfStock,
                        ItemImages = c.ItemImages,
                        SubCategoryId = c.SubCategoryId,

                    }).ToList();
                return Ok(new { Status = true, itemsList = itemsList });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }
        }
        #endregion
        #region ItemImages
        [HttpGet]
        public IActionResult getAllImagesByItemId(int ItemId)
        {
            try
            {

                var itemObj = _context.Items.Where(e => e.ItemId == ItemId).FirstOrDefault();
                if (itemObj == null)
                {
                    return Ok(new { Status = false, message = "Item Object Not Found" });
                }


                var imagsList = _context.ItemImages.Where(c => c.ItemId == ItemId).Select(c => new { ItemImageId = c.ItemImageId, ImageName = c.ImageName });
                return Ok(new { Status = true, imagsList = imagsList });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }

        }
        #endregion
        #region Add Item In Favourite
        [HttpPost]
        public IActionResult AddItemInFavourite(FavouriteVM favouriteVM)
        {
            try
            {

                var itemObj = _context.Items.Where(e => e.ItemId == favouriteVM.ItemId).FirstOrDefault();
                if (itemObj == null)
                {
                    return Ok(new { Status = false, message = "Item Object Not Found" });
                }
                var customerObj = _context.Customers.Where(e => e.CustomerId == favouriteVM.CustomerId).FirstOrDefault();
                if (customerObj == null)
                {
                    return Ok(new { Status = false, message = "Customer Object Not Found" });
                }
                var favouriteObj = _context.Favourites.Where(e => e.ItemId == favouriteVM.ItemId && e.CustomerId == favouriteVM.CustomerId).FirstOrDefault();
                if (favouriteObj != null)
                {
                    return Ok(new { Status = false, message = "Item Aleady In Favourite" });
                }
                var favourite = new Favourite()
                {
                    CustomerId = favouriteVM.CustomerId,
                    ItemId = favouriteVM.ItemId
                };
                _context.Favourites.Add(favourite);
                _context.SaveChanges();
                return Ok(new { Status = true, Favourite = favourite });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }

        }
        #region Remove Item From Favourite
        [HttpDelete]
        public IActionResult removeItemFromFavourite(int itemId, int customerId)
        {
            try
            {
                var itemObj = _context.Items.Where(e => e.ItemId == itemId).FirstOrDefault();
                if (itemObj == null)
                {
                    return Ok(new { Status = false, message = "Item Object Not Found" });
                }
                var customerObj = _context.Customers.Where(e => e.CustomerId == customerId).FirstOrDefault();
                if (customerObj == null)
                {
                    return Ok(new { Status = false, message = "Customer Object Not Found" });
                }
                var favourite = _context.Favourites.Where(e => e.CustomerId == customerId && e.ItemId == itemId).FirstOrDefault();
                if (favourite == null)
                {
                    return Ok(new { Status = false, message = "Favourite Object Not Found" });
                }
                _context.Favourites.Remove(favourite);
                _context.SaveChanges();
                return Ok(new { Status = true, Favourite = favourite });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }

        }
        #endregion
        #endregion
        #region favourites Items By Customer
        [HttpGet]
        public IActionResult getAllFavouriteItemsByCustomerId(int customerId)
        {
            try
            {

                var customerObj = _context.Customers.Where(e => e.CustomerId == customerId).FirstOrDefault();
                if (customerObj == null)
                {
                    return Ok(new { Status = false, message = "Customer Object Not Found" });
                }


                var favouriteList = _context.Favourites.Include(c => c.Item).Where(c => c.CustomerId == customerId)
                    .Select(c => new
                    {
                        FavouriteId = c.FavouriteId,
                        CustomerId = c.CustomerId,
                        ItemId = c.ItemId,
                        Shop = c.Item.Shop,
                        ItemTitleAr = c.Item.ItemTitleAr,
                        ItemTitleEn = c.Item.ItemTitleEn,
                        ItemPrice = c.Item.ItemPrice,
                        ItemDescriptionAr = c.Item.ItemDescriptionAr,
                        ItemDescriptionEn = c.Item.ItemDescriptionEn,
                        ItemImage = "Images/Item/" + c.Item.ItemImage,
                        OutOfStock = c.Item.OutOfStock,
                        ItemImages = c.Item.ItemImages,
                        SubCategoryId = c.Item.SubCategoryId,
                        CategoryId = c.Item.CategoryId,


                    }).ToList();

                return Ok(new { Status = true, favouriteList = favouriteList });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }

        }
        #endregion
        #endregion
        #endregion
        #region GetAllFunctions
        #region Countries
        [HttpGet]
        public IActionResult GetAllCountries()
        {
            try
            {
                var countryList = _context.Countries.Where(c => c.IsActive == true).OrderBy(c => c.OrderIndex)
                    .Select(c => new
                    {
                        c.CountryId,
                        c.CountryTLAR,
                        c.CountryTLEN,
                        Pic = "Images/Country/" + c.Pic,
                        c.OrderIndex
                    });
                return Ok(new { Status = true, countryList = countryList });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });

            }

        }
        #endregion
        #region Categories
        [HttpGet]
        public IActionResult GetAllCategory(int customerId)
        {
            try
            {
                var categoryList = _context.Categories.Where(c => c.IsActive == true).OrderBy(c => c.OrderIndex)
                    .Select(c => new
                    {
                        CategoryId = c.CategoryId,
                        CategoryTLAR = c.CategoryTLAR,
                        CategoryTLEN = c.CategoryTLEN,
                        CategoryPic = "Images/Category/" + c.CategoryPic,
                        OrderIndex = c.OrderIndex,
                        SubCategories = c.SubCategories,
                        Items = _context.Items
                                    .Include(c => c.ItemImages)
                                    
                                    .Where(i => i.CategoryId == c.CategoryId)
                                    .Select(x => new
                                    {
                                        x.ItemId,
                                        x.ItemPrice,
                                        x.ItemTitleAr,
                                        x.ItemTitleEn,
                                        x.ItemDescriptionAr,
                                        x.ItemDescriptionEn,
                                        x.OutOfStock,
                                        x.OrderIndex,
                                        x.SubCategoryId,
                                        x.CategoryId,
                                        ItemImage = "Images/Item/" + x.ItemImage,
                                        ItemImages = x.ItemImages,
                                        IsFavourite = _context.Favourites.Any(o => o.ItemId == x.ItemId && o.CustomerId == customerId),

                                    }).ToList()




                    }) ;
                return Ok(new { Status = true, categoryList = categoryList });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });

            }

        }
        #endregion
        #region Currencies
        [HttpGet]
        public IActionResult GetAllCurrency()
        {
            try
            {
                var currencyList = _context.Currencies
                .Where(c => c.IsActive == true)
                .Select(c => new
                {
                    CurrenyId = c.CurrencyId,
                    CurrenyTlar = c.CurrencyTLAR,
                    CurrenyTlen = c.CurrencyTLEN,
                    CurrencyPic = "images/Currecny/" + c.CurrencyPic,
                });
                return Ok(new { Status = true, currencyList = currencyList });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });

            }

        }
        #endregion
        #region PaymentMethods
        [HttpGet]
        public IActionResult GetAllPaymentMethods()
        {
            try
            {
                var paymentMehodsList = _context.PaymentMehods

                .Select(c => new
                {
                    PaymentMethodId = c.PaymentMethodId,
                    PaymentMethodName = c.PaymentMethodName,
                    PaymentMethodPic = "images/PaymentMethod/" + c.PaymentMethodPic,

                });
                return Ok(new { Status = true, paymentMehodsList = paymentMehodsList });
            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });

            }
        }
        #endregion
        #region FAQS
        [HttpGet]
        public IActionResult GetFAQList()
        {
            try
            {
                var faqsList = _context.FAQs.ToList();
                return Ok(new { Status = true, FaqsList = faqsList });

            }
            catch (Exception ex)
            {

                return Ok(new { Status = false, message = ex.Message });

            }

        }

        #endregion
        #region Shops
        [HttpGet]
        public IActionResult GetAllShopsByCountryId(int countryId)
        {
            try
            {
                var country = _context.Countries.FirstOrDefault(c => c.CountryId == countryId);
                if (country == null)
                {
                    return Ok(new { status = false, message = "Country Object Not Found" });

                }

                var shopList = _context.Shops.Where(c => c.CountryId == countryId && c.EntityTypeId == 1 &&
                c.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault().Active && c.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault().EndDate >= DateTime.Now
                && c.Country.IsActive == true).OrderBy(c => c.OrderIndex).Select(i => new
                {
                    ShopId = i.ShopId,
                    ShopTLAR = i.ShopTLAR,
                    ShopTLEN = i.ShopTLEN,
                    Address = i.Address,
                    Tele = i.Tele,
                    ShopNo = i.ShopNo,
                    Banner = i.Banner,
                    Pic = i.Pic,
                    Lat = i.Lat,
                    Lng = i.Lng,
                    Instgram = i.Instgram,
                    DescriptionTLEN = i.DescriptionTLEN,
                    DescriptionTLAR = i.DescriptionTLAR,


                    DeliveryCost = i.DeliveryCost,
                    Subscriptions = i.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault(),
                    ShopImage = i.ShopImage,

                });

                return Ok(new { status = true, shopList = shopList });
            }
            catch (Exception ex)
            {

                return Ok(new { status = false, message = ex.Message });

            }

        }
        #endregion
        #region Small Project
        [HttpGet]
        public IActionResult GetAllSmallProjectByCountryId(int countryId)
        {
            try
            {
                var country = _context.Countries.FirstOrDefault(c => c.CountryId == countryId);
                if (country == null)
                {
                    return Ok(new { status = false, message = "Country Object Not Found" });

                }

                var shopList = _context.Shops.Where(c => c.CountryId == countryId && c.EntityTypeId == 3 &&
                c.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault().Active && c.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault().EndDate >= DateTime.Now
                && c.Country.IsActive == true).OrderBy(c => c.OrderIndex).Select(i => new
                {
                    ShopId = i.ShopId,
                    ShopTLAR = i.ShopTLAR,
                    ShopTLEN = i.ShopTLEN,
                    Address = i.Address,
                    Tele = i.Tele,
                    ShopNo = i.ShopNo,
                    Banner = i.Banner,
                    Pic = i.Pic,
                    Lat = i.Lat,
                    Lng = i.Lng,
                    Instgram = i.Instgram,
                    DescriptionTLEN = i.DescriptionTLEN,
                    DescriptionTLAR = i.DescriptionTLAR,
                    DeliveryCost = i.DeliveryCost,
                    Subscriptions = i.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault(),
                    ShopImage = i.ShopImage,
                });

                return Ok(new { status = true, shopList = shopList });
            }
            catch (Exception ex)
            {

                return Ok(new { status = false, message = ex.Message });

            }

        }
        #endregion
        #region Compinies
        [HttpGet]
        public IActionResult GetCompiniesByCountryId(int countryId)
        {
            try
            {
                var country = _context.Countries.FirstOrDefault(c => c.CountryId == countryId);
                if (country == null)
                {
                    return Ok(new { status = false, message = "Country Object Not Found" });

                }

                var shopList = _context.Shops.Where(c => c.CountryId == countryId && c.EntityTypeId == 2 &&
                c.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault().Active && c.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault().EndDate >= DateTime.Now
                && c.Country.IsActive == true).OrderBy(c => c.OrderIndex).Select(i => new
                {
                    ShopId = i.ShopId,
                    ShopTLAR = i.ShopTLAR,
                    ShopTLEN = i.ShopTLEN,
                    Address = i.Address,
                    Tele = i.Tele,
                    ShopNo = i.ShopNo,
                    Banner = i.Banner,
                    Pic = i.Pic,
                    Lat = i.Lat,
                    Lng = i.Lng,
                    Instgram = i.Instgram,
                    DescriptionTLEN = i.DescriptionTLEN,
                    DescriptionTLAR = i.DescriptionTLAR,
                    DeliveryCost = i.DeliveryCost,
                    Subscriptions = i.Subscriptions.OrderByDescending(e => e.SubscriptionId).FirstOrDefault(),
                    ShopImage = i.ShopImage,

                });

                return Ok(new { status = true, shopList = shopList });
            }
            catch (Exception ex)
            {

                return Ok(new { status = false, message = ex.Message });

            }

        }
        #endregion
        #endregion
        #region Order
        #region Change Quantity
        [HttpPost]
        public IActionResult ChangeItemQuantity(int Itemid, int customerId, int Qty)
        {
            try
            {
                var customerObj = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
                if (customerObj == null)
                {
                    return Ok(new { status = false, message = "Customer Object Not Found" });

                }
                var itemObj = _context.Items.FirstOrDefault(c => c.ItemId == Itemid);
                if (itemObj == null)
                {
                    return Ok(new { status = false, message = "Item  Object Not Found" });

                }

                var shoppingCartItem = _context.ShoppingCarts.FirstOrDefault(c => c.ItemId == Itemid && c.CustomerId == customerId);
                if (shoppingCartItem == null)
                {
                    return Ok(new { status = false, message = "Shopping Cart Item  Object Not Found" });
                }
                shoppingCartItem.ItemQty = Qty;
                shoppingCartItem.ItemTotal = Qty * shoppingCartItem.ItemPrice;
                _context.Attach(shoppingCartItem).State = EntityState.Modified;
                _context.SaveChanges();

                return Ok(new { Status = true, message = "Item Qty Increased", ShoppingCartItem = shoppingCartItem });
            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });

            }


        }
        #endregion
        #region CheckOut
        [HttpPost]
        public async Task<IActionResult> CheckOut(CheckOutVM checkOutVM)
        {
            try
            {
                var customerObj = _context.Customers.FirstOrDefault(c => c.CustomerId == checkOutVM.CustomerId);
                if (customerObj == null)
                {
                    return Ok(new { status = false, message = "Customer Object Not Found" });

                }
                var customerAddressObj = _context.CustomerAddresses.FirstOrDefault(c => c.CustomerAddressId == checkOutVM.CustomerAddressId);
                if (customerAddressObj == null)
                {
                    return Ok(new { status = false, message = "Customer Address Object Not Found" });

                }
                var paymentObj = _context.PaymentMehods.FirstOrDefault(c => c.PaymentMethodId == checkOutVM.PaymentMethodId);
                if (paymentObj == null)
                {
                    return Ok(new { status = false, message = "Payment Object Not Found" });

                }
                // return Ok(new { status = true, shopList = shopList });



                double discount = 0;


                //Get Customer ShoppingCart Items List
                var customerShoppingCartList = _context.
                    ShoppingCarts.Include(s => s.Customer)
                    .Include(s => s.Item)
                    .ThenInclude(s => s.Shop)
                    .Where(c => c.CustomerId == checkOutVM.CustomerId);

                var totalOfAll = customerShoppingCartList.AsEnumerable().Sum(c => c.ItemTotal);

                // make coupon used

                Coupon coupon = null;
                coupon = _context.Coupons.FirstOrDefault(c => c.CouponId == checkOutVM.CouponId);


                //calc ordernet
                double calcOrderNet(double sumItemTotal)
                {
                    var percent = sumItemTotal / totalOfAll;

                    if (coupon == null)
                    {
                        discount = 0;
                        return sumItemTotal;
                    }
                    else if (coupon.CouponTypeId == 2)
                    {
                        discount = sumItemTotal - (float)(sumItemTotal - coupon.Amount * percent);

                        return (float)(sumItemTotal - coupon.Amount * percent);
                    }
                    else
                    {
                        var couponAmount = totalOfAll * (coupon.Amount / 100);
                        discount = sumItemTotal - (float)(sumItemTotal - couponAmount * percent);

                        return (float)(sumItemTotal - couponAmount * percent);
                    }

                }

                int maxUniqe = 1;
                var newList = _context.Orders.ToList();
                if (newList.Count > 0)
                {
                    maxUniqe = newList.Max(e => e.UniqeId);
                }


                var orders = customerShoppingCartList.AsEnumerable().GroupBy(c => c.Item.ShopId).

                Select(g => new Order
                {
                    OrderDate = DateTime.Now,
                    OrderSerial = Guid.NewGuid().ToString() + "/" + DateTime.Now.Year,
                    ShopId = g.Key,
                    CustomerId = checkOutVM.CustomerId,
                    CustomerAddressId = checkOutVM.CustomerAddressId,
                    OrderTotal = g.Sum(c => c.ItemTotal),
                    CouponId = coupon != null ? checkOutVM.CouponId : null,
                    CouponTypeId = coupon != null ? coupon.CouponTypeId : null,
                    CouponAmount = coupon != null ? (float?)coupon.Amount : null,
                    Deliverycost = _context.Shops.FirstOrDefault(s => s.ShopId == g.Key).DeliveryCost,
                    OrderNet = calcOrderNet(g.Sum(c => c.ItemTotal)) + _context.Shops.FirstOrDefault(s => s.ShopId == g.Key).DeliveryCost,
                    PaymentMethodId = checkOutVM.PaymentMethodId,
                    OrderDiscount = discount,
                    UniqeId = maxUniqe + 1

                }).ToList();



                foreach (var item in orders)
                {
                    _context.Orders.Add(item);
                    _context.SaveChanges();

                    //transfer shoppingcart to orderitems table and clear shoppingcart

                    List<OrderItem> orderItems = new List<OrderItem>();


                    foreach (var itemshop in customerShoppingCartList)
                    {
                        if (itemshop.Item.ShopId == item.ShopId)
                        {
                            OrderItem orderItem = new OrderItem
                            {
                                ItemId = (int)itemshop.ItemId,
                                ItemPrice = itemshop.ItemPrice,
                                Total = itemshop.ItemTotal,
                                ItemQuantity = itemshop.ItemQty,
                                OrderId = item.OrderId
                            };

                            _context.OrderItems.Add(orderItem);

                        }
                    }

                }
                _context.SaveChanges();
                double pointCost = 0;
                var point = _context.Points.ToList().Take(1).FirstOrDefault();
                if (point != null)
                {
                    pointCost = point.AmountOfOnePoint * customerObj.NumberOfPoints;

                }


                if (checkOutVM.PaymentMethodId == 1)
                {

                    bool Fattorahstatus = bool.Parse(_configuration["FattorahStatus"]);
                    var TestToken = _configuration["TestToken"];
                    var LiveToken = _configuration["LiveToken"];
                    if (Fattorahstatus) // fattorah live
                    {
                        var sendPaymentRequest = new
                        {

                            CustomerName = customerObj.NameAr,
                            NotificationOption = "LNK",
                            InvoiceValue = orders.Sum(e => e.OrderNet) - pointCost,
                            CallBackUrl = "http://dajeejapp.com/FattorahOrderSuccess",
                            ErrorUrl = "http://dajeejapp.com/FattorahOrderFaild",
                            UserDefinedField = orders.FirstOrDefault().UniqeId,
                            CustomerEmail = customerObj.Email

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
                            return Ok(new { status = true, Url = InvoiceRes.InvoiceURL });

                        }
                        else
                        {
                            return Ok(new { status = false, Message = FattoraRes.Message });
                        }
                    }
                    else               // fattorah test
                    {
                        var sendPaymentRequest = new
                        {

                            CustomerName = customerObj.NameAr,
                            NotificationOption = "LNK",
                            InvoiceValue = orders.Sum(e => e.OrderNet) - pointCost,
                            CallBackUrl = "http://dajeejapp.com/FattorahOrderSuccess",
                            ErrorUrl = "http://dajeejapp.com/FattorahOrderFaild",
                            UserDefinedField = orders.FirstOrDefault().UniqeId,
                            CustomerEmail = customerObj.Email

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

                            return Ok(new { status = true, Url = InvoiceRes.InvoiceURL });

                        }
                        else
                        {
                            return Ok(new { status = false, Message = FattoraRes.Message });
                        }
                    }


                }
                if (checkOutVM.PaymentMethodId == 2)
                {

                    if (orders.FirstOrDefault().CustomerId != null)
                    {
                        if (coupon != null)
                        {
                            coupon.Used = true;
                            var UpdatedCoupon = _context.Coupons.Attach(coupon);
                            UpdatedCoupon.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        }

                        _context.ShoppingCarts.RemoveRange(customerShoppingCartList);
                        _context.SaveChanges();
                        return Ok(new { status = true, Url = "http://dajeejapp.com/Thankyou" });

                    }

                }
                return Ok();

            }

            catch (Exception ex)
            {

                return Ok(new { status = false, message = ex.Message });

            }

        }
        #endregion
        #region getOrdersByCusId
        [HttpGet]
        public IActionResult GetOrderByCustomerId(int customerId)
        {
            try
            {
                var customerObj = _context.Customers.Where(e => e.CustomerId == customerId).FirstOrDefault();
                if (customerObj == null)
                {
                    return Ok(new { Status = false, message = "Customer Object Not Found" });
                }
                var listOfIOrders = _context.Orders.Where(c => c.CustomerId == customerId)
                    .Include(s => s.PaymentMehod)
                    .Select(c => new
                    {
                        OrderId = c.OrderId,

                        CustomerId = c.CustomerId,
                        CustomerAddressId = c.CustomerAddressId.Value,
                        OrderTotal = c.OrderTotal,
                        Deliverycost = c.Deliverycost,
                        IsDeliverd = c.IsDeliverd,
                        PaymentMethod = c.PaymentMehod,
                        PaymentMethodId = c.PaymentMethodId,
                        OrderSerial = c.OrderSerial,


                        ShippingAddress = _context.CustomerAddresses.FirstOrDefault(s => s.CustomerAddressId == c.CustomerAddressId),
                        OrderDiscount = c.OrderDiscount,
                        OrderDate = c.OrderDate,
                        items = _context.OrderItems
                        .Include(w => w.Item).Include(c => c.Item.Shop)
                        .Where(o => o.OrderId == c.OrderId)
                        .Select(s => new
                        {
                            ItemDetails = s.Item,
                            ItemPrice = s.ItemPrice,
                            ItemQuantity = s.ItemQuantity,
                            shop = s.Item.Shop
                        }).ToList()

                    });


                return Ok(new { Status = true, ListOfIOrders = listOfIOrders });

            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, message = ex.Message });
            }

        }

        #endregion
        #region Add Item To Shopping Cart
        [HttpPost]
        public IActionResult AddItemToCart([FromBody] ShoppingCartVM shoppingCartVM)
        {
            try
            {
                var customerObj = _context.Customers.FirstOrDefault(c => c.CustomerId == shoppingCartVM.CustomerId);
                if (customerObj == null)
                {
                    return Ok(new { status = false, message = "Customer Object Not Found" });

                }
                var itemObj = _context.Items.FirstOrDefault(c => c.ItemId == shoppingCartVM.ItemId);
                if (itemObj == null)
                {
                    return Ok(new { status = false, message = "Item  Object Not Found" });

                }
                var itemAlreadyExistInCustomerCart =
                    _context.ShoppingCarts.FirstOrDefault(s =>
                    s.ItemId == shoppingCartVM.ItemId &&
                    s.CustomerId == shoppingCartVM.CustomerId);

                var shopDeliveryCost = _context.Items.Include(i => i.Shop).FirstOrDefault(i => i.ItemId == shoppingCartVM.ItemId).Shop.DeliveryCost;

                if (shopDeliveryCost == null)
                {
                    shopDeliveryCost = 0;
                }

                if (itemAlreadyExistInCustomerCart != null)
                {
                    itemAlreadyExistInCustomerCart.ItemQty += shoppingCartVM.ItemQuantity;
                    itemAlreadyExistInCustomerCart.ItemTotal += shoppingCartVM.ItemTotal;
                    itemAlreadyExistInCustomerCart.ItemPrice = shoppingCartVM.ItemPrice;
                    itemAlreadyExistInCustomerCart.Deliverycost = shopDeliveryCost.Value;
                    _context.Attach(itemAlreadyExistInCustomerCart).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Ok(new { Status = true, message = "Item Succesfuly Added", Obj = itemAlreadyExistInCustomerCart });

                }

                ShoppingCart shoppingItemObj = new ShoppingCart()
                {
                    CustomerId = shoppingCartVM.CustomerId,
                    ItemId = shoppingCartVM.ItemId,
                    ItemPrice = shoppingCartVM.ItemPrice,
                    ItemQty = shoppingCartVM.ItemQuantity,
                    ItemTotal = shoppingCartVM.ItemTotal,
                    Deliverycost = shopDeliveryCost.Value
                };

                _context.ShoppingCarts.Add(shoppingItemObj);
                _context.SaveChanges();

                return Ok(new { Status = true, message = "Item Succesfuly Added", Obj = shoppingItemObj });

            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });

            }
        }


        #endregion
        #region Delete Item From Shopping Cart
        [HttpDelete]
        public IActionResult DeleteItemFromShoppingCart(int Itemid, int customerId)
        {
            try
            {
                var customerObj = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
                if (customerObj == null)
                {
                    return Ok(new { status = false, message = "Customer Object Not Found" });

                }
                var itemObj = _context.Items.FirstOrDefault(c => c.ItemId == Itemid);
                if (itemObj == null)
                {
                    return Ok(new { status = false, message = "Item  Object Not Found" });

                }

                var shoppingCartItem = _context.ShoppingCarts.Where(c => c.ItemId == Itemid && c.CustomerId == customerId).FirstOrDefault();
                if (shoppingCartItem == null)
                {
                    return Ok(new { status = false, message = "Shopping Cart Item Object Not Found" });

                }
                _context.ShoppingCarts.Remove(shoppingCartItem);
                _context.SaveChanges();
                return Ok(new { Status = true, message = "Item Deleted Successfully" });
            }
            catch (Exception ex)
            {

                return Ok(new { status = false, message = ex.Message });

            }


        }
        #endregion
        #region GetAllShoppingCartByCustomerId
        [HttpGet]
        public IActionResult GetShoppingCartByCustomerId(int customerId)
        {
            try
            {
                var customerObj = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
                if (customerObj == null)
                {
                    return Ok(new { status = false, message = "Customer Object Not Found" });

                }
                var shoppingCartList = _context.ShoppingCarts
                                                    .Include(s => s.Item)
                                                    .ThenInclude(s => s.Shop)
                                                    .Where(c => c.CustomerId == customerId)
                                                    .Select(c => new
                                                    {
                                                        ShoppingCartId = c.ShoppingCartId,
                                                        ItemId = c.Item.ItemId,
                                                        ItemTitleAr = c.Item.ItemTitleAr,
                                                        ItemTitleEn = c.Item.ItemTitleEn,
                                                        ItemImage = "Images/Item/" + c.Item.ItemImage,
                                                        ItemDescriptionEn = c.Item.ItemDescriptionEn,
                                                        ItemDescriptionAr = c.Item.ItemDescriptionAr,
                                                        Itemprice = c.Item.ItemPrice,
                                                        ItemQty = c.ItemQty,
                                                        ShopTLAR = c.Item.Shop.ShopTLAR,
                                                        ShopTLEN = c.Item.Shop.ShopTLEN,
                                                        ShopId = c.Item.Shop.ShopId
                                                    }).ToList();

                double totalDeliveryCost = 0;

                var tdc = shoppingCartList.GroupBy(c => c.ShopId).

                Select(g => new
                {
                    TotalDeliveryCost = _context.Shops.SingleOrDefault(s => s.ShopId == g.Key).DeliveryCost

                }).ToList();

                foreach (var item in tdc)
                {
                    totalDeliveryCost += item.TotalDeliveryCost.Value;
                }

                return Ok(new { Status = true, ShoppingCartList = shoppingCartList, TotalDeliveryCost = totalDeliveryCost });

            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });

            }

        }
        #endregion
        #endregion
        #region Copuns
        [HttpGet]
        public IActionResult ValidateCopoun([FromQuery] string CouponSerial)
        {
            try
            {
                var coupon = _context.Coupons.FirstOrDefault(c => c.Serial == CouponSerial);
                if (coupon == null)
                {
                    return Ok(new { Success = false, message = "Coupon Not Exist" });
                }
                if (
                    (DateTime.Now.Date >= coupon.IssueDate.Date) &&
                    (DateTime.Now.Date <= coupon.ExpirationDate.Date) &&
                    (coupon.Used != true))
                {
                    return Ok(new { Success = true, message = "Coupon Exist", CouponId = coupon.CouponId, CouponTypeId = coupon.CouponTypeId, CouponAmount = coupon.Amount });
                }

                return Ok(new { Success = false, message = "Coupon Not Exist" });
            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });

            }


        }

        #endregion
        #region Get All Social Links
        [HttpGet]
        public IActionResult getAllSocialLinks()
        {
            try
            {
                string adminRoleId = _db.Roles.Where(e => e.Name == "Admin").FirstOrDefault().Id;
                var UserAdminId = _db.UserRoles.Where(e => e.RoleId == adminRoleId).FirstOrDefault().UserId;
                var user = _userManager.Users.Where(e => e.Id == UserAdminId).FirstOrDefault();
                var SocialLinks = _context.SoicialMidiaLinks.ToList().Take(1).FirstOrDefault();

                if (SocialLinks == null)
                {
                    return Ok(new { Success = false, message = "Object Not Exist" });
                }
                var SocialObj = new SocialLinksVm()
                {
                    Instgramlink = SocialLinks.Instgramlink,
                    WhatsApplink = SocialLinks.WhatsApplink,
                    YoutubeLink = SocialLinks.WhatsApplink,
                    TwitterLink = SocialLinks.TwitterLink,
                    LinkedInlink = SocialLinks.LinkedInlink,
                    facebooklink = SocialLinks.facebooklink,
                    id = SocialLinks.id,
                    AdminEmail = user.Email,
                    AdminPhone = user.PhoneNumber

                };

                return Ok(new { Success = true, SocialLinks = SocialObj });
            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });

            }


        }

        #endregion
        #region GetCostOfCustomerPoints
        [HttpGet]
        public IActionResult getCostOfCustomerPoints(int customerId)
        {
            try
            {
                var customerObj = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
                if (customerObj == null)
                {
                    return Ok(new { status = false, message = "Customer Object Not Found" });

                }
                var point = _context.Points.ToList().Take(1).FirstOrDefault();
                if (point == null)
                {
                    return Ok(new { Success = false, message = "Point Object Not Found" });
                }

                double cost = point.AmountOfOnePoint * customerObj.NumberOfPoints;

                return Ok(new { Success = true, NumberOfPoints = customerObj.NumberOfPoints, Cost = cost });
            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });

            }


        }

        #endregion
        #region GetSliders
        [HttpGet]
        public IActionResult getSlidersByCountryId(int countryId)
        {
            try
            {
                var countryObj = _context.Countries.FirstOrDefault(c => c.CountryId == countryId);
                if (countryObj == null)
                {
                    return Ok(new { status = false, message = "Country  Object Not Found" });

                }
                var sliderList = _context.Sliders.Where(e => e.CountryId == countryId && e.IsActive == true).OrderBy(e => e.OrderIndex).Select(c => new
                {
                    CountryId = c.CountryId,
                    SliderId = c.SliderId,
                    IsActive = c.IsActive,
                    OrderIndex = c.OrderIndex,
                    Pic = "Images/Slider/" + c.Pic,

                }).ToList();



                return Ok(new { Status = true, SliderList = sliderList });
            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });

            }


        }

        #endregion

        [HttpPost]
        public IActionResult ADDListItemsToShoppingcart([FromBody] List<ShoppingCartVM> Items)
        {
            try
            {
                for (int i = 0; i < Items.Count; i++)
                {

                    var customerObj = _context.Customers.FirstOrDefault(c => c.CustomerId == Items[i].CustomerId);
                    if (customerObj == null)
                    {
                        return Ok(new { status = false, message = $"Customer Object No {i}  Not Found" });

                    }
                    var itemObj = _context.Items.FirstOrDefault(c => c.ItemId == Items[i].ItemId);
                    if (itemObj == null)
                    {
                        return Ok(new { status = false, message = $"Item  Object No {i} Not Found" });

                    }
                    var itemAlreadyExistInCustomerCart =
                        _context.ShoppingCarts.FirstOrDefault(s =>
                        s.ItemId == Items[i].ItemId &&
                        s.CustomerId == Items[i].CustomerId);
                    var itemId = Items[i].ItemId;
                    var shopDeliveryCost = _context.Items.Include(i => i.Shop).FirstOrDefault(i => i.ItemId == itemId).Shop.DeliveryCost;

                    if (shopDeliveryCost == null)
                    {
                        shopDeliveryCost = 0;
                    }

                    if (itemAlreadyExistInCustomerCart != null)
                    {
                        itemAlreadyExistInCustomerCart.ItemQty += Items[i].ItemQuantity;
                        itemAlreadyExistInCustomerCart.ItemTotal += Items[i].ItemTotal;
                        itemAlreadyExistInCustomerCart.ItemPrice = Items[i].ItemPrice;
                        itemAlreadyExistInCustomerCart.Deliverycost = shopDeliveryCost.Value;
                        _context.Attach(itemAlreadyExistInCustomerCart).State = EntityState.Modified;
                        _context.SaveChanges();

                    }
                    else
                    {
                        ShoppingCart shoppingItemObj = new ShoppingCart()
                        {
                            CustomerId = Items[i].CustomerId,
                            ItemId = Items[i].ItemId,
                            ItemPrice = Items[i].ItemPrice,
                            ItemQty = Items[i].ItemQuantity,
                            ItemTotal = Items[i].ItemTotal,
                            Deliverycost = shopDeliveryCost.Value
                        };

                        _context.ShoppingCarts.Add(shoppingItemObj);
                        _context.SaveChanges();
                    }
                }
                return Ok(new { status = true, message = "List Added Successfully" });

            }
            catch (Exception ex)
            {
                return Ok(new { status = false, message = ex.Message });

            }
        }
        #region Notifications
        [HttpPost]
        public IActionResult AddPublicDevice([FromBody] PublicDevice model)
        {

            try
            {
                var publicDevice = _context.PublicDevices.FirstOrDefault(c => c.DeviceId == model.DeviceId);
                if (publicDevice != null)
                {
                    publicDevice.CountryId = model.CountryId;
                    publicDevice.IsAndroiodDevice = model.IsAndroiodDevice;
                    _context.PublicDevices.Update(publicDevice);
                    _context.SaveChanges();
                    return Ok(new { Status = true, Message = "device Edited Successfully" });
                }
                _context.PublicDevices.Add(model);
                _context.SaveChanges();
                return Ok(new { Status = true, Message = "Device Added Successfully" });

            }
            catch (Exception ex)
            {
                return Ok(new { Status = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetPublicNotificationByDeviceId([FromQuery] string deviceId)

        {
            try
            {
                var publicDevice = _context.PublicDevices.FirstOrDefault(c => c.DeviceId == deviceId);
                var NotificationList = _context.PublicNotificationDevices.Include(c => c.PublicNotification).Where(c => c.PublicDeviceId == publicDevice.PublicDeviceId);


                return Ok(new { Status = true, NotificationList = NotificationList });
            }
            catch (Exception e)
            {

                return Ok(new { Status = false, Message = e.Message });

            }


        }
        [HttpDelete]
        public IActionResult DeletePublicNotification([FromQuery] int publicNotificationDeviceId)
        {
            try
            {
                var model = _context.PublicNotificationDevices.FirstOrDefault(c => c.PublicNotificationDeviceId == publicNotificationDeviceId);
                _context.Remove(model);
                _context.SaveChanges();
                return Ok(new { Status = true, Message = "Deleted Successfully" });
            }
            catch (Exception e)
            {

                return Ok(new { Status = false, Message = e.Message });

            }


        }
        [HttpGet]
        public IActionResult MakeNotificationIsRead([FromQuery] int PublicNotificationDeviceId)
        {

            try
            {
                var model = _context.PublicNotificationDevices.Find(PublicNotificationDeviceId);
                if (model == null)
                {
                    return Ok(new { Status = false, Message = "Notification Obj Not Found" });


                }
                model.IsRead = true;
                _context.PublicNotificationDevices.Update(model);
                _context.SaveChanges();

                return Ok(new { Status = true, Message = "deviceId Added " });

            }
            catch (Exception e)
            {

                return Ok(new { Status = false, Message = e.Message });

            }
        }
        #endregion
        #region Banners
        [HttpGet]
        public IActionResult GetBanners(int countryId)
        {

            var bannerList = _context.Banners
                .Where(b => (b.IsActive == true) && (b.CountryId == countryId))
                .OrderBy(c => c.OrderIndex)
                .Select(c => new
                {
                    BannerId = c.BannerId,
                    Pic = "images/Banner/" + c.Pic,
                    OrderIndex = c.OrderIndex,
                    SliderTypeId = c.EntityTypeNotifyId,
                    EntityId = c.EntityId
                });
            return Ok(new { Status = true, BannerList = bannerList });

        }
        #endregion
        #region Collections
        [HttpGet]
        public IActionResult GetCollectionsItems(int customerId, int countryId)
        {

            var lst = _context.Collections.Where(c => c.IsActive == true).OrderBy(c => c.Source)
                .Select(c => new
                {
                    c.CollectionTitleEn,
                    c.CollectionTitleAr,
                    c.CollectionId,
                    c.Source,
                    Items = _context.CollectionItems
                    .Include(c => c.Item)
                    .ThenInclude(c => c.Shop)
                    .Where(i => i.CollectionId == c.CollectionId && i.Item.Shop.CountryId == countryId)
                    .Select(x => new
                    {
                        x.ItemId,
                        x.Item.ItemPrice,
                        x.Item.ItemTitleAr,
                        x.Item.ItemTitleEn,
                        x.Item.ItemDescriptionAr,
                        x.Item.ItemDescriptionEn,
                        x.Item.OutOfStock,
                        x.Item.OrderIndex,
                        x.Item.SubCategoryId,
                        x.Item.CategoryId,
                        ItemImage = "Images/Item/" + x.Item.ItemImage,
                        ItemImages = x.Item.ItemImages,
                        IsFavourite = _context.Favourites.Any(o => o.ItemId == x.ItemId && o.CustomerId == customerId),

                    }).ToList()
                }).OrderBy(c => c.Source).ToList();

            var collectionList = new List<Object>();

            foreach (var item in lst)
            {
                if (item.Items.Count() > 0)
                {
                    collectionList.Add(item);
                }
            }
            return Ok(new { Status = true, CollectionList = collectionList });


        }
        [HttpGet]
        public IActionResult GetCollections()
        {
            try
            {
                var collectionList = _context.Collections.OrderBy(c => c.Source).ToList();
                return Ok(new { Status = true, CollectionList = collectionList });
            }
            catch (Exception e)
            {
                return Ok(new { Status = false, Message = e.Message });
            }

        }
        [HttpGet]
        public IActionResult GetCollectionItemByCollectionId(int CollectionId)
        {
            try
            {
                var collectionList = _context.CollectionItems.Include(c => c.Item).Where(i => i.CollectionId == CollectionId).Select(x => new
                {
                    x.ItemId,
                    x.Item.ItemPrice,
                    x.Item.ItemTitleAr,
                    x.Item.ItemTitleEn,
                    x.Item.ItemDescriptionAr,
                    x.Item.ItemDescriptionEn,
                    x.Item.OutOfStock,
                    x.Item.OrderIndex,
                    x.Item.SubCategoryId,
                    x.Item.CategoryId,
                    ItemImage = "Images/Item/" + x.Item.ItemImage,
                    ItemImages = x.Item.ItemImages

                }).ToList();



                return Ok(new { Status = true, CollectionList = collectionList });
            }
            catch (Exception e)
            {
                return Ok(new { Status = false, Message = e.Message });
            }

        }

        #endregion

    }

}
