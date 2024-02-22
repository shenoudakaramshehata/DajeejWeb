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
    public class ShopImagesController : Controller
    {
        private DajeejContext _context;

        public ShopImagesController(DajeejContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions) {
            var shopimages = _context.ShopImages.Select(i => new {
                i.ShopImagesId,
                i.ImageName,
                i.ShopId
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "ShopImagesId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(shopimages, loadOptions));
        }
        [HttpGet]
        public async Task<object> GetImagesForShop([FromQuery] int id)
        {
            var productimages = _context.ShopImages.Where(p => p.ShopId == id).Select(i => new {
                i.ShopImagesId,
                i.ImageName,
                i.ShopId
            });

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "ImageId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return productimages;
        }

        [HttpPost]
        public async Task<int> RemoveImageById([FromQuery] int id)
        {
            var shopPic = _context.ShopImages.FirstOrDefault(p => p.ShopImagesId == id);
            _context.ShopImages.Remove(shopPic);
            _context.SaveChanges();

            return id;
        }
        [HttpPost]
        public async Task<IActionResult> Post(string values) {
            var model = new ShopImage();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if(!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.ShopImages.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.ShopImagesId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values) {
            var model = await _context.ShopImages.FirstOrDefaultAsync(item => item.ShopImagesId == key);
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
            var model = await _context.ShopImages.FirstOrDefaultAsync(item => item.ShopImagesId == key);

            _context.ShopImages.Remove(model);
            await _context.SaveChangesAsync();
        }


        [HttpGet]
        public async Task<IActionResult> ShopsLookup(DataSourceLoadOptions loadOptions) {
            var lookup = from i in _context.Shops
                         orderby i.ShopTLAR
                         select new {
                             Value = i.ShopId,
                             Text = i.ShopTLAR
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        private void PopulateModel(ShopImage model, IDictionary values) {
            string SHOP_IMAGES_ID = nameof(ShopImage.ShopImagesId);
            string IMAGE_NAME = nameof(ShopImage.ImageName);
            string SHOP_ID = nameof(ShopImage.ShopId);

            if(values.Contains(SHOP_IMAGES_ID)) {
                model.ShopImagesId = Convert.ToInt32(values[SHOP_IMAGES_ID]);
            }

            if(values.Contains(IMAGE_NAME)) {
                model.ImageName = Convert.ToString(values[IMAGE_NAME]);
            }

            if(values.Contains(SHOP_ID)) {
                model.ShopId = Convert.ToInt32(values[SHOP_ID]);
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