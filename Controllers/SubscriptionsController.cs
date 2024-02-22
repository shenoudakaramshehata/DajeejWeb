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

namespace Dajeej.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SubscriptionsController : Controller
    {
        private DajeejContext _context;

        public SubscriptionsController(DajeejContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var subscriptions = _context.Subscriptions.Select(i => new {
                i.SubscriptionId,
                i.ShopId,
                i.PlanId,
                i.StartDate,
                i.EndDate,
                i.Active
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "Id" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(subscriptions, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> GetByShopId(DataSourceLoadOptions loadOptions, string id)
        {
            var subscriptions = _context.Subscriptions
                .Where(s => s.ShopId == int.Parse(id))
                .Select(i => new {
                i.SubscriptionId,
                i.ShopId,
                i.PlanId,
                i.StartDate,
                i.EndDate,
                i.Active
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "Id" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(subscriptions, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Subscription();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Subscriptions.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.SubscriptionId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.Subscriptions.FirstOrDefaultAsync(item => item.SubscriptionId == key);
            if(model == null)
                return StatusCode(409, "Object not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(int key) {
            var model = await _context.Subscriptions.FirstOrDefaultAsync(item => item.SubscriptionId == key);

            _context.Subscriptions.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> ShopLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Shops
                         orderby i.ShopTLAR
                         select new {
                             Value = i.ShopId,
                             Text = i.ShopTLAR
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> PlansLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Plans
                         orderby i.ArabicTitle
                         select new {
                             Value = i.PlanId,
                             Text = i.ArabicTitle
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(Subscription model, IDictionary values) {
            string SUBSCRIPTION_ID = nameof(Subscription.SubscriptionId);
            string SHOP_ID = nameof(Subscription.ShopId);
            string PLAN_ID = nameof(Subscription.PlanId);
            string START_DATE = nameof(Subscription.StartDate);
            string END_DATE = nameof(Subscription.EndDate);
            string ACTIVE = nameof(Subscription.Active);

            if(values.Contains(SUBSCRIPTION_ID)) {
                model.SubscriptionId = Convert.ToInt32(values[SUBSCRIPTION_ID]);
            }

            if(values.Contains(SHOP_ID)) {
                model.ShopId = Convert.ToInt32(values[SHOP_ID]);
            }

            if(values.Contains(PLAN_ID)) {
                model.PlanId = Convert.ToInt32(values[PLAN_ID]);
            }


            if(values.Contains(START_DATE)) {
                model.StartDate = Convert.ToDateTime(values[START_DATE]);
            }

            if(values.Contains(END_DATE)) {
                model.EndDate = Convert.ToDateTime(values[END_DATE]);
            }

            if(values.Contains(ACTIVE)) {
                model.Active = Convert.ToBoolean(values[ACTIVE]);
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState) {
            var messages = new List<string>();

            foreach(var entry in modelState) {
                foreach(var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}