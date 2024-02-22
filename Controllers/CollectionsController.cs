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
    public class CollectionsController : Controller
    {
        private DajeejContext _context;

        public CollectionsController(DajeejContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var collections = _context.Collections.Select(i => new {
                i.CollectionId,
                i.CollectionTitleAr,
                i.CollectionTitleEn,
                i.Source,
                i.IsActive
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "CollectionId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(collections, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Collection();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Collections.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.CollectionId });
        }

        [HttpGet]
        public async Task<IActionResult> CollectionsLookup(DataSourceLoadOptions loadOptions)
        {
            var lookup = from i in _context.Collections
                         orderby i.CollectionId
                         select new
                         {
                             id = i.CollectionId,
                             text = i.CollectionTitleEn
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }


        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.Collections.FirstOrDefaultAsync(item => item.CollectionId == key);
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
            var model = await _context.Collections.FirstOrDefaultAsync(item => item.CollectionId == key);

            _context.Collections.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(Collection model, IDictionary values) {
            string COLLECTION_ID = nameof(Collection.CollectionId);
            string COLLECTION_TITLE_AR = nameof(Collection.CollectionTitleAr);
            string COLLECTION_TITLE_EN = nameof(Collection.CollectionTitleEn);
            string SOURCE = nameof(Collection.Source);
            string IS_ACTIVE = nameof(Collection.IsActive);

            if(values.Contains(COLLECTION_ID)) {
                model.CollectionId = Convert.ToInt32(values[COLLECTION_ID]);
            }

            if(values.Contains(COLLECTION_TITLE_AR)) {
                model.CollectionTitleAr = Convert.ToString(values[COLLECTION_TITLE_AR]);
            }

            if(values.Contains(COLLECTION_TITLE_EN)) {
                model.CollectionTitleEn = Convert.ToString(values[COLLECTION_TITLE_EN]);
            }

            if(values.Contains(SOURCE)) {
                model.Source = Convert.ToInt32(values[SOURCE]);
            }

            if(values.Contains(IS_ACTIVE)) {
                model.IsActive = Convert.ToBoolean(values[IS_ACTIVE]);
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