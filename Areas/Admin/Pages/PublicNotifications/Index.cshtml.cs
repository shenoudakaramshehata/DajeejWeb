using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Dajeej.Data;
using Dajeej.Models;
using NToastNotify;

namespace Dajeej.Areas.Admin.Pages.PublicNotifications
{
    public class IndexModel : PageModel
    {
        private DajeejContext _context;
        private readonly IToastNotification _toastNotification;


        public IndexModel(DajeejContext context,  IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;

        }

        [BindProperty(SupportsGet = true)]
        public List<PublicNotification> PublicNotificationLst { get; set; }

        public void OnGet()
        {
            try
            {

                PublicNotificationLst = _context.PublicNotifications.ToList();

                foreach (var item in PublicNotificationLst)
                {



                    if (item.EntityTypeNotifyId == 1)
                    {
                        item.EntityNameAr = _context.Items.FirstOrDefault(c => c.ItemId == item.EntityId)?.ItemTitleAr;
                        item.EntityNameEn = _context.Items.FirstOrDefault(c => c.ItemId == item.EntityId)?.ItemTitleEn;
                    }
                    if (item.EntityTypeNotifyId == 2)
                    {
                        item.EntityNameAr = _context.Shops.FirstOrDefault(c => c.ShopId == item.EntityId)?.ShopTLAR;
                        item.EntityNameEn = _context.Shops.FirstOrDefault(c => c.ShopId == item.EntityId)?.ShopTLEN;
                    }
                   


                }
            }
            catch (Exception)
            {

                _toastNotification.AddErrorToastMessage("Something went wrong");

            }
        }
        }
}
