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
    public class FAQsController : Controller
    {
        private DajeejContext _context;

        public FAQsController(DajeejContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var faqs = _context.FAQs.Select(i => new {
                i.FAQId,
                i.QuestionAr,
                i.AnswerAr,
                i.QuestionEn,
                i.AnswerEn
            });

            // If you work with a large amount of data, consider specifying the PaginateViaPrimaryKey and PrimaryKey properties.
            // In this case, keys and data are loaded in separate queries. This can make the SQL execution plan more efficient.
            // Refer to the topic https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "FAQId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(faqs, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new FAQ();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.FAQs.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.FAQId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.FAQs.FirstOrDefaultAsync(item => item.FAQId == key);
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
            var model = await _context.FAQs.FirstOrDefaultAsync(item => item.FAQId == key);

            _context.FAQs.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(FAQ model, IDictionary values) {
            string FAQID = nameof(FAQ.FAQId);
            string QUESTION_AR = nameof(FAQ.QuestionAr);
            string ANSWER_AR = nameof(FAQ.AnswerAr);
            string QUESTION_EN = nameof(FAQ.QuestionEn);
            string ANSWER_EN = nameof(FAQ.AnswerEn);

            if(values.Contains(FAQID)) {
                model.FAQId = Convert.ToInt32(values[FAQID]);
            }

            if(values.Contains(QUESTION_AR)) {
                model.QuestionAr = Convert.ToString(values[QUESTION_AR]);
            }

            if(values.Contains(ANSWER_AR)) {
                model.AnswerAr = Convert.ToString(values[ANSWER_AR]);
            }

            if(values.Contains(QUESTION_EN)) {
                model.QuestionEn = Convert.ToString(values[QUESTION_EN]);
            }

            if(values.Contains(ANSWER_EN)) {
                model.AnswerEn = Convert.ToString(values[ANSWER_EN]);
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