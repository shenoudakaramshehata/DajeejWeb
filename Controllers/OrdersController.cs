using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Dajeej.Data;
using Dajeej.Models;
using Microsoft.AspNetCore.Identity;

namespace Dajeej.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class OrdersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private DajeejContext _context;

        public OrdersController(DajeejContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Shop)
                .Include(o => o.PaymentMehod).Select(i => new Order {
                    OrderId = i.OrderId,
                    IsDeliverd = i.IsDeliverd,
                    OrderDiscount= i.OrderDiscount,
                    Deliverycost = i.Deliverycost,
                    Customer = i.Customer,
                    CustomerId = i.CustomerId,
                    Shop = i.Shop,
                    ShopId = i.ShopId,
                    PaymentMehod = i.PaymentMehod,
                    PaymentMethodId = i.PaymentMethodId,
                   

                });
            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "OrderId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(order, loadOptions));
        }
        [HttpGet]
        public async Task<IActionResult> GetOrdersforshop(DataSourceLoadOptions loadOptions)
        {
            var shopId = _userManager.FindByNameAsync(User.Identity.Name).Result.EntityId;

            var order = _context.Orders
                .Include(s => s.Customer)
                .Include(s => s.PaymentMehod)
                .Include(s => s.Shop)
                .Where(i => i.ShopId == shopId).Select(i => new Order
                {
                    OrderId = i.OrderId,
                    IsDeliverd = i.IsDeliverd,
                    OrderDiscount = i.OrderDiscount,
                    Deliverycost = i.Deliverycost,
                    Customer = i.Customer,
                    CustomerId = i.CustomerId,
                    Shop = i.Shop,
                    ShopId = i.ShopId,
                    PaymentMehod = i.PaymentMehod,
                    PaymentMethodId = i.PaymentMethodId,

                });



            return Json(await DataSourceLoader.LoadAsync(order, loadOptions));
        }


        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {
            var model = new Order();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if (!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Orders.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.OrderId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int? key, string values)
        {
            var model = await _context.Orders.FirstOrDefaultAsync(item => item.OrderId == key);
            if (model == null)
                return StatusCode(409, "Object not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if (!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(int? key)
        {
            var model = await _context.Orders.FirstOrDefaultAsync(item => item.OrderId == key);

            _context.Orders.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> CustomerLookup(DataSourceLoadOptions loadOptions)
        {
            var lookup = from i in _context.Customers
                         orderby i.NameEn
                         select new
                         {
                             Value = i.CustomerId,
                             Text = i.NameEn
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }
        [HttpGet]
        public async Task<IActionResult> ShopsLookup(DataSourceLoadOptions loadOptions)
        {
            var lookup = from i in _context.Shops
                         orderby i.ShopTLAR
                         select new
                         {
                             Value = i.ShopId,
                             Text = i.ShopTLAR
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }
        [HttpGet]
        public async Task<IActionResult> PaymentsLookup(DataSourceLoadOptions loadOptions)
        {
            var lookup = from i in _context.PaymentMehods
                         orderby i.PaymentMethodName
                         select new
                         {
                             Value = i.PaymentMethodId,
                             Text = i.PaymentMethodName
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }



        private void PopulateModel(Order model, IDictionary values)
        {
            string ORDER_ID = nameof(Order.OrderId);
            //  string ITEM_ID = nameof(Order.ItemId);
            string CUSTOMER_ID = nameof(Order.CustomerId);
            //string QUANTITY = nameof(Order.Quantity);
            string CUSTOMER_ADDRESS_ID = nameof(Order.CustomerAddressId);
            string IS_DELIVERD = nameof(Order.IsDeliverd);
            string NOTES = nameof(Order.Notes);
            //string ORDER_INDEX = nameof(Order.OrderIndex);

            if (values.Contains(ORDER_ID))
            {
                model.OrderId = (int)(values[ORDER_ID] != null ? Convert.ToInt32(values[ORDER_ID]) : (int?)null);
            }

            //if(values.Contains(ITEM_ID)) {
            //     model.ItemId = values[ITEM_ID] != null ? Convert.ToInt32(values[ITEM_ID]) : (int?)null;
            // }

            if (values.Contains(CUSTOMER_ID))
            {
                model.CustomerId = values[CUSTOMER_ID] != null ? Convert.ToInt32(values[CUSTOMER_ID]) : (int?)null;
            }

            //if(values.Contains(QUANTITY)) {
            //    model.Quantity = values[QUANTITY] != null ? Convert.ToInt32(values[QUANTITY]) : (int?)null;
            //}

            if (values.Contains(CUSTOMER_ADDRESS_ID))
            {
                model.CustomerAddressId = values[CUSTOMER_ADDRESS_ID] != null ? Convert.ToInt32(values[CUSTOMER_ADDRESS_ID]) : (int?)null;
            }

            if (values.Contains(IS_DELIVERD))
            {
                model.IsDeliverd = values[IS_DELIVERD] != null ? Convert.ToBoolean(values[IS_DELIVERD]) : (bool?)null;
            }

            if (values.Contains(NOTES))
            {
                model.Notes = Convert.ToString(values[NOTES]);
            }

            //if (values.Contains(ORDER_INDEX))
            //{
            //    model.OrderIndex = values[ORDER_INDEX] != null ? Convert.ToInt32(values[ORDER_INDEX]) : (int?)null;
            //}
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState)
        {
            var messages = new List<string>();

            foreach (var entry in modelState)
            {
                foreach (var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}