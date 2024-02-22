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
    public class CurrenciesController : Controller
    {
        private DajeejContext _context;

        public CurrenciesController(DajeejContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var curreny = _context.Currencies.Select(i => new {
                i.CurrencyId,
                i.CurrencyTLAR,
                i.CurrencyTLEN,
                i.CurrencyPic,
                i.IsActive
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "CurrenyId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(curreny, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Currency();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Currencies.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.CurrencyId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.Currencies.FirstOrDefaultAsync(item => item.CurrencyId == key);
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
            var model = await _context.Currencies.FirstOrDefaultAsync(item => item.CurrencyId == key);

            _context.Currencies.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(Currency model, IDictionary values) {
            string CURRENY_ID = nameof(Currency.CurrencyId);
            string CURRENY_TLAR = nameof(Currency.CurrencyTLAR);
            string CURRENY_TLEN = nameof(Currency.CurrencyTLEN);
            string CURRENCY_PIC = nameof(Currency.CurrencyPic);
            string IS_ACTIVE = nameof(Currency.IsActive);

            if(values.Contains(CURRENY_ID)) {
                model.CurrencyId = Convert.ToInt32(values[CURRENY_ID]);
            }

            if(values.Contains(CURRENY_TLAR)) {
                model.CurrencyTLAR = Convert.ToString(values[CURRENY_TLAR]);
            }

            if(values.Contains(CURRENY_TLEN)) {
                model.CurrencyTLEN = Convert.ToString(values[CURRENY_TLEN]);
            }

            if(values.Contains(CURRENCY_PIC)) {
                model.CurrencyPic = Convert.ToString(values[CURRENCY_PIC]);
            }

            if(values.Contains(IS_ACTIVE)) {
                model.IsActive = values[IS_ACTIVE] != null ? Convert.ToBoolean(values[IS_ACTIVE]) : (bool?)null;
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