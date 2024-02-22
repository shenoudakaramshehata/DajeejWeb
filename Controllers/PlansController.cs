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
    public class PlansController : Controller
    {
        private DajeejContext _context;

        public PlansController(DajeejContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var plans = _context.Plans.Select(i => new {
                i.PlanId,
                i.ArabicTitle,
                i.EnglishTitle,
                i.Period,
                i.NoOfItems,
                i.Price,
                i.VipCollection,
                i.Reports,
                i.Dashboard,
                i.AdzBanners,
                i.CountryId
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "Id" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(plans, loadOptions));
        }


        [HttpGet]
        public async Task<IActionResult> GetPlansForIndexPage(DataSourceLoadOptions loadOptions)
        {
            var plans = _context.Plans.Include(p => p.Country).Select(i => new {
                i.PlanId,
                i.ArabicTitle,
                i.EnglishTitle,
                i.Period,
                i.NoOfItems,
                i.Price,
                i.VipCollection,
                i.Reports,
                i.Dashboard,
                i.AdzBanners,
                i.CountryId,
                CountryTlen = i.Country.CountryTLAR
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "Id" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(plans, loadOptions));
        }


        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Plan();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Plans.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.PlanId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.Plans.FirstOrDefaultAsync(item => item.PlanId == key);
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
            var model = await _context.Plans.FirstOrDefaultAsync(item => item.PlanId == key);

            _context.Plans.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> CountryLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Countries
                         orderby i.CountryTLAR
                         select new {
                             Value = i.CountryId,
                             Text = i.CountryTLAR
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> GetPlansByCountryLookup(DataSourceLoadOptions loadOptions, int id)
        {
            var country = await _context.Shops.FirstOrDefaultAsync(s => s.ShopId == id);
            var countryId = country.CountryId;

            var lookup = from i in _context.Plans
                         where i.CountryId == countryId
                         orderby i.PlanId
                         select new
                         {
                             Value = i.PlanId,
                             Text = i.EnglishTitle
                         };

            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }
        private void PopulateModel(Plan model, IDictionary values) {
            string ID = nameof(Plan.PlanId);
            string ARABIC_TITLE = nameof(Plan.ArabicTitle);
            string ENGLISH_TITLE = nameof(Plan.EnglishTitle);
            string PERIOD = nameof(Plan.Period);
            string NO_OF_ITEMS = nameof(Plan.NoOfItems);
            string PRICE = nameof(Plan.Price);
            string VIP_COLLECTION = nameof(Plan.VipCollection);
            string REPORTS = nameof(Plan.Reports);
            string DASHBOARD = nameof(Plan.Dashboard);
            string ADZ_BANNERS = nameof(Plan.AdzBanners);
            string COUNTRY_ID = nameof(Plan.CountryId);

            if(values.Contains(ID)) {
                model.PlanId = Convert.ToInt32(values[ID]);
            }

            if(values.Contains(ARABIC_TITLE)) {
                model.ArabicTitle = Convert.ToString(values[ARABIC_TITLE]);
            }

            if(values.Contains(ENGLISH_TITLE)) {
                model.EnglishTitle = Convert.ToString(values[ENGLISH_TITLE]);
            }

            if(values.Contains(PERIOD)) {
                model.Period = Convert.ToInt32(values[PERIOD]);
            }

            if(values.Contains(NO_OF_ITEMS)) {
                model.NoOfItems = Convert.ToInt32(values[NO_OF_ITEMS]);
            }

            if(values.Contains(PRICE)) {
                model.Price = Convert.ToDouble(values[PRICE], CultureInfo.InvariantCulture);
            }

            if(values.Contains(VIP_COLLECTION)) {
                model.VipCollection = Convert.ToBoolean(values[VIP_COLLECTION]);
            }

            if(values.Contains(REPORTS)) {
                model.Reports = Convert.ToBoolean(values[REPORTS]);
            }

            if(values.Contains(DASHBOARD)) {
                model.Dashboard = Convert.ToBoolean(values[DASHBOARD]);
            }

            if(values.Contains(ADZ_BANNERS)) {
                model.AdzBanners = Convert.ToBoolean(values[ADZ_BANNERS]);
            }

            if(values.Contains(COUNTRY_ID)) {
                model.CountryId =  Convert.ToInt32(values[COUNTRY_ID]);
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