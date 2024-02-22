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
    public class CategoriesController : Controller
    {
        private DajeejContext _context;

        public CategoriesController(DajeejContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var category = _context.Categories.Select(i => new {
                i.CategoryId,
                i.CategoryTLAR,
                i.CategoryTLEN,
                i.CategoryPic,
                i.CategoryIcon,
                i.IsActive,
                i.OrderIndex
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "CategoryId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(category, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Category();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Categories.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.CategoryId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.Categories.FirstOrDefaultAsync(item => item.CategoryId == key);
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
            var model = await _context.Categories.FirstOrDefaultAsync(item => item.CategoryId == key);

            _context.Categories.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(Category model, IDictionary values) {
            string CATEGORY_ID = nameof(Category.CategoryId);
            string CATEGORY_TLAR = nameof(Category.CategoryTLAR);
            string CATEGORY_TLEN = nameof(Category.CategoryTLEN);
            string CATEGORY_PIC = nameof(Category.CategoryPic);
            string CATEGORY_ICON = nameof(Category.CategoryIcon);
            string IS_ACTIVE = nameof(Category.IsActive);
            string ORDER_INDEX = nameof(Category.OrderIndex);

            if(values.Contains(CATEGORY_ID)) {
                model.CategoryId = Convert.ToInt32(values[CATEGORY_ID]);
            }

            if(values.Contains(CATEGORY_TLAR)) {
                model.CategoryTLAR = Convert.ToString(values[CATEGORY_TLAR]);
            }

            if(values.Contains(CATEGORY_TLEN)) {
                model.CategoryTLEN = Convert.ToString(values[CATEGORY_TLEN]);
            }

            if(values.Contains(CATEGORY_PIC)) {
                model.CategoryPic = Convert.ToString(values[CATEGORY_PIC]);
            }

            if(values.Contains(CATEGORY_ICON)) {
                model.CategoryIcon = Convert.ToString(values[CATEGORY_ICON]);
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