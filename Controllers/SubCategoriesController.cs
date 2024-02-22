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
    public class SubCategoriesController : Controller
    {
        private DajeejContext _context;

        public SubCategoriesController(DajeejContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var subcategory = _context.SubCategories.Select(i => new {
                i.SubCategoryId,
                i.CategoryId,
                i.SubCategoryTLAR,
                i.SubCategoryTLEN,
                i.SubCategoryPic,
                i.IsActive,
                i.OrderIndex
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "SubCategoryId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(subcategory, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new SubCategory();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.SubCategories.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.SubCategoryId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.SubCategories.FirstOrDefaultAsync(item => item.SubCategoryId == key);
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
        public async Task<IActionResult> Delete(int key) {

            var model = await _context.SubCategories.Include(c => c.Items).FirstOrDefaultAsync(item => item.SubCategoryId == key);

            if(model.Items.Count() > 0)
            {
                return StatusCode(409, "You cannot delete this subcategory because it has items");
            }

            _context.SubCategories.Remove(model);
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> CategoryLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Categories
                         orderby i.CategoryTLAR
                         select new {
                             Value = i.CategoryId,
                             Text = i.CategoryTLAR
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(SubCategory model, IDictionary values) {
            string SUB_CATEGORY_ID = nameof(SubCategory.SubCategoryId);
            string CATEGORY_ID = nameof(SubCategory.CategoryId);
            string SUB_CATEGORY_TLAR = nameof(SubCategory.SubCategoryTLAR);
            string SUB_CATEGORY_TLEN = nameof(SubCategory.SubCategoryTLEN);
            string SUB_CATEGORY_PIC = nameof(SubCategory.SubCategoryPic);
            string IS_ACTIVE = nameof(SubCategory.IsActive);
            string ORDER_INDEX = nameof(SubCategory.OrderIndex);

            if(values.Contains(SUB_CATEGORY_ID)) {
                model.SubCategoryId = Convert.ToInt32(values[SUB_CATEGORY_ID]);
            }

            if(values.Contains(CATEGORY_ID)) {
                model.CategoryId = Convert.ToInt32(values[CATEGORY_ID]);
            }

            if(values.Contains(SUB_CATEGORY_TLAR)) {
                model.SubCategoryTLAR = Convert.ToString(values[SUB_CATEGORY_TLAR]);
            }

            if(values.Contains(SUB_CATEGORY_TLEN)) {
                model.SubCategoryTLEN = Convert.ToString(values[SUB_CATEGORY_TLEN]);
            }

            if(values.Contains(SUB_CATEGORY_PIC)) {
                model.SubCategoryPic = Convert.ToString(values[SUB_CATEGORY_PIC]);
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