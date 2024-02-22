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
    public class PageContentsController : Controller
    {
        private DajeejContext _context;

        public PageContentsController(DajeejContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var pagecontent = _context.PageContents.Select(i => new {
                i.PageContentId,
                i.PageTitleAr,
                i.ContentAr,
                i.PageTitleEn,
                i.ContentEn
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "PageContentId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(pagecontent, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new PageContent();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.PageContents.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.PageContentId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.PageContents.FirstOrDefaultAsync(item => item.PageContentId == key);
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
            var model = await _context.PageContents.FirstOrDefaultAsync(item => item.PageContentId == key);

            _context.PageContents.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(PageContent model, IDictionary values) {
            string PAGE_CONTENT_ID = nameof(PageContent.PageContentId);
            string PAGE_TITLE_AR = nameof(PageContent.PageTitleAr);
            string CONTENT_AR = nameof(PageContent.ContentAr);
            string PAGE_TITLE_EN = nameof(PageContent.PageTitleEn);
            string CONTENT_EN = nameof(PageContent.ContentEn);

            if(values.Contains(PAGE_CONTENT_ID)) {
                model.PageContentId = Convert.ToInt32(values[PAGE_CONTENT_ID]);
            }

            if(values.Contains(PAGE_TITLE_AR)) {
                model.PageTitleAr = Convert.ToString(values[PAGE_TITLE_AR]);
            }

            if(values.Contains(CONTENT_AR)) {
                model.ContentAr = Convert.ToString(values[CONTENT_AR]);
            }

            if(values.Contains(PAGE_TITLE_EN)) {
                model.PageTitleEn = Convert.ToString(values[PAGE_TITLE_EN]);
            }

            if(values.Contains(CONTENT_EN)) {
                model.ContentEn = Convert.ToString(values[CONTENT_EN]);
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