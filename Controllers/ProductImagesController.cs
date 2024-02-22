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
    public class ProductImagesController : Controller
    {
        private DajeejContext _context;

        public ProductImagesController(DajeejContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var productimages = _context.ItemImages.Select(i => new {
                i.ItemImageId,
                i.ImageName,
                i.ItemId
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "ImageId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(productimages, loadOptions));
        }

        [HttpGet]
        public async Task<object> GetImagesForItem([FromQuery] int id)
        {
            var productimages = _context.ItemImages.Where(p => p.ItemId == id).Select(i => new {
                i.ItemImageId,
                i.ImageName,
                i.ItemId
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "ImageId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return productimages;
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new ItemImage();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.ItemImages.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.ItemImageId });
        }

        [HttpPost]
        public async Task<int> RemoveImageById([FromQuery]int id)
        {
            var itemPic = _context.ItemImages.FirstOrDefault(p => p.ItemImageId == id);
            _context.ItemImages.Remove(itemPic);
            _context.SaveChanges();

            return id;
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.ItemImages.FirstOrDefaultAsync(item => item.ItemImageId == key);
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
            var model = await _context.ItemImages.FirstOrDefaultAsync(item => item.ItemImageId == key);

            _context.ItemImages.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> ItemsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Items
                         orderby i.ItemTitleAr
                         select new {
                             Value = i.ItemId,
                             Text = i.ItemTitleAr
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(ItemImage model, IDictionary values) {
            string Item_IMAGE_ID = nameof(ItemImage.ItemImageId);
            string IMAGE_NAME = nameof(ItemImage.ImageName);
            string ITEM_ID = nameof(ItemImage.ItemId);

            if(values.Contains(Item_IMAGE_ID)) {
                model.ItemImageId = Convert.ToInt32(values[Item_IMAGE_ID]);
            }

            if(values.Contains(IMAGE_NAME)) {
                model.ImageName = Convert.ToString(values[IMAGE_NAME]);
            }

            if(values.Contains(ITEM_ID)) {
                model.ItemId =  Convert.ToInt32(values[ITEM_ID]);
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