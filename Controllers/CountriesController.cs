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
   
    public class CountriesController : Controller
    {
        private DajeejContext _context;

        public CountriesController(DajeejContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var country = _context.Countries.Select(i => new {
                i.CountryId,
                i.CountryTLAR,
                i.CountryTLEN,
                i.Pic,
                i.IsActive,
                i.OrderIndex
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "CountryId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(country, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            try
            {
                var model = new Country();
                var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
                PopulateModel(model, valuesDict);

                if (!TryValidateModel(model))
                    return BadRequest(GetFullErrorMessage(ModelState));

                var result = _context.Countries.Add(model);
                await _context.SaveChangesAsync();

                return Json(new { result.Entity.CountryId });
            }
            catch (Exception ex)
            {

                return Ok(ex.InnerException);
            }
           
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.Countries.FirstOrDefaultAsync(item => item.CountryId == key);
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
            var model = await _context.Countries.FirstOrDefaultAsync(item => item.CountryId == key);

            _context.Countries.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(Country model, IDictionary values) {
            string COUNTRY_ID = nameof(Country.CountryId);
            string COUNTRY_TLAR = nameof(Country.CountryTLAR);
            string COUNTRY_TLEN = nameof(Country.CountryTLEN);
            string PIC = nameof(Country.Pic);
            string IS_ACTIVE = nameof(Country.IsActive);
            string ORDER_INDEX = nameof(Country.OrderIndex);

            if(values.Contains(COUNTRY_ID)) {
                model.CountryId = Convert.ToInt32(values[COUNTRY_ID]);
            }

            if(values.Contains(COUNTRY_TLAR)) {
                model.CountryTLAR = Convert.ToString(values[COUNTRY_TLAR]);
            }

            if(values.Contains(COUNTRY_TLEN)) {
                model.CountryTLEN = Convert.ToString(values[COUNTRY_TLEN]);
            }

            if(values.Contains(PIC)) {
                model.Pic = Convert.ToString(values[PIC]);
            }

            if(values.Contains(IS_ACTIVE)) {
                model.IsActive = values[IS_ACTIVE] != null ? Convert.ToBoolean(values[IS_ACTIVE]) : (bool?)null;
            }

            if(values.Contains(ORDER_INDEX)) {
                model.OrderIndex = values[ORDER_INDEX] != null ? Convert.ToInt32(values[ORDER_INDEX]) : (int?)null;
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