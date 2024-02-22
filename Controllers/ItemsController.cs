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
using Microsoft.AspNetCore.Identity;

namespace Dajeej.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ItemsController : Controller
    {
        private DajeejContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ItemsController(DajeejContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetByShopId(DataSourceLoadOptions loadOptions, string id)
        {
            var Items = _context.Items.Where(s => s.ShopId == int.Parse(id)).Select(i => new {
                i.ItemId,
                i.CategoryId,
                i.SubCategoryId,
                i.ShopId,
                i.ItemTitleAr,
                i.ItemTitleEn,
                i.ItemImage,
                i.ItemDescriptionAr,
                i.ItemDescriptionEn,
                i.ItemPrice,
                i.IsActive,
                i.OrderIndex,

                i.OutOfStock,
                //i.CountryId
            });

            return Json(await DataSourceLoader.LoadAsync(Items, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var items = _context.Items.Select(i => new {
                i.ItemId,
                i.CategoryId,
                i.SubCategoryId,
                i.ShopId,
                i.ItemTitleAr,
                i.ItemTitleEn,
                i.ItemImage,
                i.ItemDescriptionAr,
                i.ItemDescriptionEn,
                i.ItemPrice,
                i.IsActive,
                i.OrderIndex,
               
                i.OutOfStock,
                //i.CountryId
            });

            return Json(await DataSourceLoader.LoadAsync(items, loadOptions));
        }
        [HttpGet]
        public async Task<IActionResult> GetItemsForSpecificShop(DataSourceLoadOptions loadOptions)
        {
            var shopId = _userManager.FindByNameAsync(User.Identity.Name).Result.EntityId;

            var items = _context.Items.Where(i => i.ShopId == shopId);

            // If underlying data is a large SQL table, specify PrimaryKey and PaginateViaPrimaryKey.
            // This can make SQL execution plans more efficient.
            // For more detailed information, please refer to this discussion: https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/336.
            // loadOptions.PrimaryKey = new[] { "ItemId" };
            // loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(items, loadOptions));
        }


        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {
            var model = new Item();
            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if (!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            var result = _context.Items.Add(model);
            await _context.SaveChangesAsync();

            return Json(new { result.Entity.ItemId });
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values)
        {
            var model = await _context.Items.FirstOrDefaultAsync(item => item.ItemId == key);
            if (model == null)
                return StatusCode(409, "Object not found");

            var valuesDict = JsonConvert.DeserializeObject<IDictionary>(values);
            PopulateModel(model, valuesDict);

            if (!TryValidateModel(model))
                return BadRequest(GetFullErrorMessage(ModelState));

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task Delete(int key)
        {
            var model = await _context.Items.FirstOrDefaultAsync(item => item.ItemId == key);

            _context.Items.Remove(model);
            await _context.SaveChangesAsync();
        }



        [HttpGet]
        public async Task<IActionResult> CategoryLookup(DataSourceLoadOptions loadOptions)
        {
            var lookup = from i in _context.Categories
                         orderby i.CategoryTLAR
                         select new
                         {
                             Value = i.CategoryId,
                             Text = i.CategoryTLAR
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> ShopLookup(DataSourceLoadOptions loadOptions)
        {
            var lookup = from i in _context.Shops
                         orderby i.ShopTLAR
                         select new
                         {
                             Value = i.ShopId,
                             Text = i.ShopTLAR
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> SubCategoryLookup(DataSourceLoadOptions loadOptions)
        {

            var lookup = _context.SubCategories.OrderBy(c => c.OrderIndex);

            //var lookup = from i in _context.SubCategories
            //             orderby i.SubCategoryTLAR
            //             select new
            //             {
            //                 Value = i.SubCategoryId,
            //                 Text = i.SubCategoryTLAR
            //             };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }
        [HttpGet]
        public async Task<IActionResult> SubCategoryLookupByCategory(DataSourceLoadOptions loadOptions, int? CategoryId)
        {
            //var lookup = _context.SubCategories.OrderBy(c => c.OrderIndex);

            var lookup = from i in _context.SubCategories where i.CategoryId==CategoryId
                         orderby i.OrderIndex
                         select new
                         {
                             Value = i.SubCategoryId,
                             Text = i.SubCategoryTLAR
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }


        [HttpGet]
        public async Task<IActionResult> SubCategoryLookup2(DataSourceLoadOptions loadOptions)
        {
            //var lookup = _context.SubCategory.OrderBy(c => c.OrderIndex);

            var lookup = from i in _context.SubCategories
                         orderby i.SubCategoryTLAR
                         select new
                         {
                             Value = i.SubCategoryId,
                             Text = i.SubCategoryTLAR
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }

        [HttpGet]
        public async Task<IActionResult> SubCategoryLookupForEdit(DataSourceLoadOptions loadOptions)
        {

            var lookup = from i in _context.SubCategories
                         orderby i.SubCategoryTLAR
                         select new
                         {
                             Value = i.SubCategoryId,
                             Text = i.SubCategoryTLAR
                         };
            return Json(await DataSourceLoader.LoadAsync(lookup, loadOptions));
        }
        private void PopulateModel(Item model, IDictionary values)
        {
            string ITEM_ID = nameof(Item.ItemId);
            string CATEGORY_ID = nameof(Item.CategoryId);
            string SUB_CATEGORY_ID = nameof(Item.SubCategoryId);
            string SHOP_ID = nameof(Item.ShopId);
            string ITEM_TITLE_AR = nameof(Item.ItemTitleAr);
            string ITEM_TITLE_EN = nameof(Item.ItemTitleEn);
            string ITEM_IMAGE = nameof(Item.ItemImage);
            string ITEM_DESCRIPTION_AR = nameof(Item.ItemDescriptionAr);
            string ITEM_DESCRIPTION_EN = nameof(Item.ItemDescriptionEn);
            string ITEM_PRICE = nameof(Item.ItemPrice);
            string IS_ACTIVE = nameof(Item.IsActive);
            string ORDER_INDEX = nameof(Item.OrderIndex);

            if (values.Contains(ITEM_ID))
            {
                model.ItemId = Convert.ToInt32(values[ITEM_ID]);
            }

            if (values.Contains(CATEGORY_ID))
            {
                model.CategoryId = Convert.ToInt32(values[CATEGORY_ID]) ;
            }

            if (values.Contains(SUB_CATEGORY_ID))
            {
                model.SubCategoryId = values[SUB_CATEGORY_ID] != null ? Convert.ToInt32(values[SUB_CATEGORY_ID]) : (int?)null;
            }

            if (values.Contains(SHOP_ID))
            {
                model.ShopId =  Convert.ToInt32(values[SHOP_ID]);
            }

            if (values.Contains(ITEM_TITLE_AR))
            {
                model.ItemTitleAr = Convert.ToString(values[ITEM_TITLE_AR]);
            }
            if (values.Contains(ITEM_TITLE_EN))
            {
                model.ItemTitleEn = Convert.ToString(values[ITEM_TITLE_EN]);
            }

            if (values.Contains(ITEM_IMAGE))
            {
                model.ItemImage = Convert.ToString(values[ITEM_IMAGE]);
            }

            if (values.Contains(ITEM_DESCRIPTION_AR))
            {
                model.ItemDescriptionAr = Convert.ToString(values[ITEM_DESCRIPTION_AR]);
            }
            if (values.Contains(ITEM_DESCRIPTION_EN))
            {
                model.ItemDescriptionEn = Convert.ToString(values[ITEM_DESCRIPTION_EN]);
            }

            if (values.Contains(ITEM_PRICE))
            {
                model.ItemPrice = Convert.ToDouble(values[ITEM_PRICE]);
            }

            //if(values.Contains(IS_FAVOURITE)) {
            //    model.IsFavourite = values[IS_FAVOURITE] != null ? Convert.ToBoolean(values[IS_FAVOURITE]) : (bool?)null;
            //}

            if (values.Contains(IS_ACTIVE))
            {
                model.IsActive = values[IS_ACTIVE] != null ? Convert.ToBoolean(values[IS_ACTIVE]) : (bool?)null;
            }

            if (values.Contains(ORDER_INDEX))
            {
                model.OrderIndex = values[ORDER_INDEX] != null ? Convert.ToInt32(values[ORDER_INDEX]) : (int?)null;
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState)
        {
            var messages = new List<string>();

            foreach (var entry in modelState)
            {
                foreach (var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}