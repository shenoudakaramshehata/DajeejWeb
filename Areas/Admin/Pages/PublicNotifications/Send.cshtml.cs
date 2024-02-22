using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dajeej.Data;
using Dajeej.Models;
using Dajeej.Entities.Notification;
using NToastNotify;

namespace Dajeej.Areas.Admin.Pages.PublicNotifications
{
    public class SendModel : PageModel
    {
        private DajeejContext _context;
        private readonly INotificationService _notificationService;
        private readonly IToastNotification _toastNotification;

        public SendModel(DajeejContext context, INotificationService notificationService, IToastNotification toastNotification)
        {
            _context = context;
            _notificationService = notificationService;
            _toastNotification = toastNotification;


        }
        [BindProperty]
        public PublicNotification publicNotification { get; set; }

        public IActionResult OnGetAsync(int id)
        {
            try
            {
                publicNotification = _context.PublicNotifications.Include(c => c.EntityTypeNotify).Include(c => c.Country).Where(c => c.PublicNotificationId == id).FirstOrDefault();

                if (publicNotification == null)
                {
                    return Redirect("../SomethingwentError");
                }

                if (publicNotification.EntityTypeNotifyId == 1)
                {
                    publicNotification.EntityNameAr = _context.Items.FirstOrDefault(c => c.ItemId == publicNotification.EntityId)?.ItemTitleAr;
                    publicNotification.EntityNameEn = _context.Items.FirstOrDefault(c => c.ItemId == publicNotification.EntityId)?.ItemTitleEn;
                }
                if (publicNotification.EntityTypeNotifyId == 2)
                {
                    publicNotification.EntityNameAr = _context.Shops.FirstOrDefault(c => c.ShopId == publicNotification.EntityId)?.ShopTLAR;
                    publicNotification.EntityNameEn = _context.Shops.FirstOrDefault(c => c.ShopId == publicNotification.EntityId)?.ShopTLEN;
                }
                

        }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");

            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {



            try
            {
                var publicNotificationModel = _context.PublicNotifications.Find(id);
                if (publicNotificationModel != null)
                {

                    var PublicDeviceList = _context.PublicDevices.Where(c => c.CountryId == publicNotificationModel.CountryId).ToList();
                    foreach (var item in PublicDeviceList)
                    {

                        var notificationModel = new NotificationModel();
                        notificationModel.DeviceId = item.DeviceId;
                        notificationModel.IsAndroiodDevice = item.IsAndroiodDevice;
                        notificationModel.Title = publicNotificationModel.Title;
                        notificationModel.Body = publicNotificationModel.Body;
                        notificationModel.IsAndroiodDevice = item.IsAndroiodDevice;
                        notificationModel.EntityId = publicNotificationModel.EntityId;
                        notificationModel.EntityTypeNotifyId= publicNotificationModel.EntityTypeNotifyId;
                        var result = await _notificationService.SendNotification(notificationModel);
                        if (result.IsSuccess)
                        {
                            var publicNotificationDeviceExiest = _context.PublicNotificationDevices.Any(c => c.PublicNotificationId == publicNotificationModel.PublicNotificationId
                             && c.PublicDeviceId == item.PublicDeviceId);
                            if (!publicNotificationDeviceExiest)
                            {
                                var publicNotificationDevice = new PublicNotificationDevice()
                                {
                                    PublicNotificationId = publicNotificationModel.PublicNotificationId,
                                    PublicDeviceId = item.PublicDeviceId,
                                    IsRead = false
                                };
                                _context.Add(publicNotificationDevice);
                                _context.SaveChanges();
                            }
                           

                        }
                    }

                }
                _toastNotification.AddSuccessToastMessage("Notification Sent successfully");
            }
            catch (Exception)

            {
                _toastNotification.AddErrorToastMessage("Something Went Error");

                return Page();

            }

            return RedirectToPage("./Index");
        }
    }
}
