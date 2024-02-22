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
    public class PointsController : Controller
    {
        private DajeejContext _context;

        public PointsController(DajeejContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var points = _context.Points.Select(i => new {
                i.PointId,
                i.ForEach,
                i.NumberOfPoints,
                i.AmountOfOnePoint
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "PointId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(points, loadOptions));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new Point();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Points.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.PointId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.Points.FirstOrDefaultAsync(item => item.PointId == key);
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
            var model = await _context.Points.FirstOrDefaultAsync(item => item.PointId == key);

            _context.Points.Remove(model);
            await _context.SaveChangesAsync();
        }


        private void PopulateModel(Point model, IDictionary values) {
            string POINT_ID = nameof(Point.PointId);
            string FOR_EACH = nameof(Point.ForEach);
            string NUMBER_OF_POINTS = nameof(Point.NumberOfPoints);
            string AMOUNT_OF_ONE_POINT = nameof(Point.AmountOfOnePoint);

            if(values.Contains(POINT_ID)) {
                model.PointId = Convert.ToInt32(values[POINT_ID]);
            }

            if(values.Contains(FOR_EACH)) {
                model.ForEach = Convert.ToInt32(values[FOR_EACH]);
            }

            if(values.Contains(NUMBER_OF_POINTS)) {
                model.NumberOfPoints = Convert.ToInt32(values[NUMBER_OF_POINTS]);
            }

            if(values.Contains(AMOUNT_OF_ONE_POINT)) {
                model.AmountOfOnePoint = Convert.ToDouble(values[AMOUNT_OF_ONE_POINT], CultureInfo.InvariantCulture);
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