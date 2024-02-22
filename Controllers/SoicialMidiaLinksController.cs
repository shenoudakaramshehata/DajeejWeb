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
    public class SoicialMidiaLinksController : Controller
    {
        private DajeejContext _context;

        public SoicialMidiaLinksController(DajeejContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var soicialmidialink = _context.SoicialMidiaLinks.Select(i => new {
                i.id,
                i.facebooklink,
                i.WhatsApplink,
                i.LinkedInlink,
                i.Instgramlink,
                i.TwitterLink
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "id" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(soicialmidialink, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new SoicialMidiaLink();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.SoicialMidiaLinks.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.id });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.SoicialMidiaLinks.FirstOrDefaultAsync(item => item.id == key);
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
            var model = await _context.SoicialMidiaLinks.FirstOrDefaultAsync(item => item.id == key);

            _context.SoicialMidiaLinks.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(SoicialMidiaLink model, IDictionary values) {
            string ID = nameof(SoicialMidiaLink.id);
            string FACEBOOKLINK = nameof(SoicialMidiaLink.facebooklink);
            string WHATS_APPLINK = nameof(SoicialMidiaLink.WhatsApplink);
            string LINKED_INLINK = nameof(SoicialMidiaLink.LinkedInlink);
            string INSTGRAMLINK = nameof(SoicialMidiaLink.Instgramlink);
            string TWITTER_LINK = nameof(SoicialMidiaLink.TwitterLink);

            if(values.Contains(ID)) {
                model.id = Convert.ToInt32(values[ID]);
            }

            if(values.Contains(FACEBOOKLINK)) {
                model.facebooklink = Convert.ToString(values[FACEBOOKLINK]);
            }

            if(values.Contains(WHATS_APPLINK)) {
                model.WhatsApplink = Convert.ToString(values[WHATS_APPLINK]);
            }

            if(values.Contains(LINKED_INLINK)) {
                model.LinkedInlink = Convert.ToString(values[LINKED_INLINK]);
            }

            if(values.Contains(INSTGRAMLINK)) {
                model.Instgramlink = Convert.ToString(values[INSTGRAMLINK]);
            }

            if(values.Contains(TWITTER_LINK)) {
                model.TwitterLink = Convert.ToString(values[TWITTER_LINK]);
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