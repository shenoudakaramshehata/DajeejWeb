using DevExtreme.AspNet.Mvc;
using Dajeej.Models;
using Dajeej.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dajeej.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StatisticsController : Controller
    {
        private  DajeejContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StatisticsController(DajeejContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public object GetTopTenItems(DataSourceLoadOptions loadOptions)
        {
            var OrderPerDriversDashboardViewModel = _context.OrderItems.GroupBy(c => c.ItemId).

                Select(g => new
                {

                    Day = _context.Items.FirstOrDefault(r => r.ItemId == g.Key).ItemTitleEn,

                    Oranges = g.Sum(s => s.ItemQuantity)

                }).OrderByDescending(r => r.Oranges).Take(10);

            return OrderPerDriversDashboardViewModel;


        }
        [HttpGet]
        public object GetTopTenItemsForShop(DataSourceLoadOptions loadOptions)
        {
            var OrderPerDriversDashboardViewModel = _context.OrderItems.Include(i => i.Item)
                .Where(s => s.Item.ShopId == _userManager.GetUserAsync(User).Result.EntityId)
                .GroupBy(c => c.ItemId).

                Select(g => new
                {

                    Day = _context.Items.FirstOrDefault(r => r.ItemId == g.Key).ItemTitleEn,

                    Oranges = g.Sum(s => s.ItemQuantity)

                }).OrderByDescending(r => r.Oranges).Take(10);

            return OrderPerDriversDashboardViewModel;


        }

        [HttpGet]
        public object GetTopTenItemsRevenue(DataSourceLoadOptions loadOptions)
        {
            var OrderPerDriversDashboardViewModel = _context.OrderItems.GroupBy(c => c.ItemId).

                Select(g => new
                {

                    Day = _context.Items.FirstOrDefault(r => r.ItemId == g.Key).ItemTitleEn,

                    Oranges = g.Sum(s => s.Total)

                }).OrderByDescending(r => r.Oranges).Take(10);

            return OrderPerDriversDashboardViewModel;


        }

        [HttpGet]
        public object GetTopTenItemsRevenueForShop(DataSourceLoadOptions loadOptions)
        {
            var OrderPerDriversDashboardViewModel = _context.OrderItems
                .Include(i => i.Item)
                 .Where(s => s.Item.ShopId == _userManager.GetUserAsync(User).Result.EntityId)
                .GroupBy(c => c.ItemId).

                Select(g => new
                {

                    Day = _context.Items.FirstOrDefault(r => r.ItemId == g.Key).ItemTitleEn,

                    Oranges = g.Sum(s => s.Total)

                }).OrderByDescending(r => r.Oranges).Take(10);

            return OrderPerDriversDashboardViewModel;


        }
        [HttpGet]
        public object GetTopTenShops(DataSourceLoadOptions loadOptions)
        {
            var OrderPerDriversDashboardViewModel = _context.Orders.GroupBy(c => c.ShopId).

                Select(g => new
                {

                    Day = _context.Shops.FirstOrDefault(r => r.ShopId == g.Key).ShopTLEN,

                    Oranges = g.Count()

                }).OrderByDescending(r => r.Oranges).Take(10);

            return OrderPerDriversDashboardViewModel;


        }
        [HttpGet]
        public object GetTopTenShopsRevenue(DataSourceLoadOptions loadOptions)
        {
            var OrderPerDriversDashboardViewModel = _context.Orders.GroupBy(c => c.ShopId).

                Select(g => new
                {

                    Day = _context.Shops.FirstOrDefault(r => r.ShopId == g.Key).ShopTLEN,

                    Oranges = g.Sum(o => o.OrderNet)

                }).OrderByDescending(r => r.Oranges).Take(10);

            return OrderPerDriversDashboardViewModel;


        }
        [HttpGet]
        public object GetTopCategories(DataSourceLoadOptions loadOptions)
        {
            var OrderPerDriversDashboardViewModel = _context.OrderItems
                .Include(s => s.Item)
                .GroupBy(c => c.Item.CategoryId).

                Select(g => new
                {

                    Day = _context.Categories.FirstOrDefault(r => r.CategoryId == g.Key).CategoryTLEN,

                    Oranges = g.Sum(s => s.ItemQuantity)

                }).OrderByDescending(r => r.Oranges).Take(10);

            return OrderPerDriversDashboardViewModel;

        }
        [HttpGet]
        public object GetTopCategoriesForShop(DataSourceLoadOptions loadOptions)
        {
            var OrderPerDriversDashboardViewModel = _context.OrderItems
                .Include(s => s.Item)
                .Where(s => s.Item.ShopId == _userManager.GetUserAsync(User).Result.EntityId)
                .GroupBy(c => c.Item.CategoryId).

                Select(g => new
                {

                    Day = _context.Categories.FirstOrDefault(r => r.CategoryId == g.Key).CategoryTLEN,

                    Oranges = g.Sum(s => s.ItemQuantity)

                }).OrderByDescending(r => r.Oranges).Take(10);

            return OrderPerDriversDashboardViewModel;

        }
        [HttpGet]
        public object GetTopCategoriesRevenue(DataSourceLoadOptions loadOptions)
        {
            var OrderPerDriversDashboardViewModel = _context.OrderItems
                .Include(s => s.Item)
                .GroupBy(c => c.Item.CategoryId).

                Select(g => new
                {

                    Day = _context.Categories.FirstOrDefault(r => r.CategoryId == g.Key).CategoryTLEN,

                    Oranges = g.Sum(s => s.Total)

                }).OrderByDescending(r => r.Oranges).Take(10);

            return OrderPerDriversDashboardViewModel;

        }
        [HttpGet]
        public object GetTopCategoriesRevenueForShop(DataSourceLoadOptions loadOptions)
        {
            var OrderPerDriversDashboardViewModel = _context.OrderItems
                .Include(s => s.Item)
                .Where(s => s.Item.ShopId == _userManager.GetUserAsync(User).Result.EntityId)
                .GroupBy(c => c.Item.CategoryId).

                Select(g => new
                {

                    Day = _context.Categories.FirstOrDefault(r => r.CategoryId == g.Key).CategoryTLEN,

                    Oranges = g.Sum(s => s.Total)

                }).OrderByDescending(r => r.Oranges).Take(10);

            return OrderPerDriversDashboardViewModel;

        }
        [HttpGet]
        public object GetTopSubCategories(DataSourceLoadOptions loadOptions)
        {
            var OrderPerDriversDashboardViewModel = _context.OrderItems
                .Include(s => s.Item)
                .GroupBy(c => c.Item.SubCategoryId).

                Select(g => new
                {

                    Day = _context.SubCategories.FirstOrDefault(r => r.SubCategoryId == g.Key).SubCategoryTLEN,

                    Oranges = g.Sum(s => s.ItemQuantity)

                }).OrderByDescending(r => r.Oranges).Take(10);

            return OrderPerDriversDashboardViewModel;


        }
        [HttpGet]
        public object GetTopSubCategoriesForShop(DataSourceLoadOptions loadOptions)
        {
            var OrderPerDriversDashboardViewModel = _context.OrderItems
                .Include(s => s.Item)
                .Where(s => s.Item.ShopId == _userManager.GetUserAsync(User).Result.EntityId)
                .GroupBy(c => c.Item.SubCategoryId).

                Select(g => new
                {

                    Day = _context.SubCategories.FirstOrDefault(r => r.SubCategoryId == g.Key).SubCategoryTLEN,

                    Oranges = g.Sum(s => s.ItemQuantity)

                }).OrderByDescending(r => r.Oranges).Take(10);

            return OrderPerDriversDashboardViewModel;


        }
        [HttpGet]
        public object GetTopSubCategoriesRevenue(DataSourceLoadOptions loadOptions)
        {
            var OrderPerDriversDashboardViewModel = _context.OrderItems
                .Include(s => s.Item)
                .GroupBy(c => c.Item.SubCategoryId).

                Select(g => new
                {

                    Day = _context.SubCategories.FirstOrDefault(r => r.SubCategoryId == g.Key).SubCategoryTLEN,

                    Oranges = g.Sum(s => s.Total)

                }).OrderByDescending(r => r.Oranges).Take(10);

            return OrderPerDriversDashboardViewModel;


        }
        [HttpGet]
        public object GetTopSubCategoriesRevenueForShop(DataSourceLoadOptions loadOptions)
        {
            var OrderPerDriversDashboardViewModel = _context.OrderItems
                .Include(s => s.Item)
                .Where(s => s.Item.ShopId == _userManager.GetUserAsync(User).Result.EntityId)
                .GroupBy(c => c.Item.SubCategoryId).

                Select(g => new
                {

                    Day = _context.SubCategories.FirstOrDefault(r => r.SubCategoryId == g.Key).SubCategoryTLEN,

                    Oranges = g.Sum(s => s.Total)

                }).OrderByDescending(r => r.Oranges).Take(10);

            return OrderPerDriversDashboardViewModel;


        }
        [HttpGet]
        public object GetDailyOrder(DataSourceLoadOptions loadOptions)
        {
            var dailyOrder = _context.Orders
                .Where(o => o.OrderDate.Year == DateTime.Now.Year)
                .GroupBy(c => c.OrderDate.Date).

                Select(g => new
                {

                    day = g.Key,

                    sales = g.Count()

                }).OrderBy(r => r.day).ThenBy(r => DateTime.Now.Month);

            return dailyOrder;


        }
        [HttpGet]
        public object GetDailyOrderForShop(DataSourceLoadOptions loadOptions)
        {
            var dailyOrder = _context.Orders
                .Where(o => (o.ShopId == _userManager.GetUserAsync(User).Result.EntityId) && (o.OrderDate.Year == DateTime.Now.Year))
                .GroupBy(c => c.OrderDate.Date).

                Select(g => new
                {

                    day = g.Key,

                    sales = g.Count()

                }).OrderBy(r => r.day).ThenBy(r => DateTime.Now.Month);

            return dailyOrder;


        }
        [HttpGet]
        public object GetDailyRevenue(DataSourceLoadOptions loadOptions)
        {
            var dailyOrder = _context.Orders
                .Where(o => o.OrderDate.Year == DateTime.Now.Year)
                .GroupBy(c => c.OrderDate.Date).

                Select(g => new
                {

                    day = g.Key,

                    sales = g.Sum(s => s.OrderNet)

                }).OrderBy(r => r.day).ThenBy(r => DateTime.Now.Month);

            return dailyOrder;


        }
        [HttpGet]
        public object GetDailyRevenueForShop(DataSourceLoadOptions loadOptions)
        {
            var dailyOrder = _context.Orders
                .Where(o => (o.ShopId == _userManager.GetUserAsync(User).Result.EntityId) && (o.OrderDate.Year == DateTime.Now.Year))
                .GroupBy(c => c.OrderDate.Date).

                Select(g => new
                {

                    day = g.Key,

                    sales = g.Sum(s => s.OrderNet)

                }).OrderBy(r => r.day).ThenBy(r => DateTime.Now.Month);

            return dailyOrder;


        }
        [HttpGet]
        public object GetMonthlyOrder(DataSourceLoadOptions loadOptions)
        {
            var dailyOrder = _context.Orders
                .Where(o => o.OrderDate.Year == DateTime.Now.Year)
                .GroupBy(c => c.OrderDate.Date.Month).

                Select(g => new
                {

                    day = g.Key,

                    sales = g.Count()

                }).OrderByDescending(r => r.day);

            return dailyOrder;


        }
        [HttpGet]

        public object GetMonthlyOrderForShop(DataSourceLoadOptions loadOptions)
        {
            var dailyOrder = _context.Orders
                .Where(o => (o.ShopId == _userManager.GetUserAsync(User).Result.EntityId) && (o.OrderDate.Year == DateTime.Now.Year))
                .GroupBy(c => c.OrderDate.Date.Month).

                Select(g => new
                {

                    day = g.Key,

                    sales = g.Count()

                }).OrderByDescending(r => r.day);

            return dailyOrder;


        }
        [HttpGet]

        public object GetMonthlyRevenue(DataSourceLoadOptions loadOptions)
        {
            var dailyOrder = _context.Orders
                .Where(o => o.OrderDate.Year == DateTime.Now.Year)
                .GroupBy(c => c.OrderDate.Date.Month).

                Select(g => new
                {

                    day = g.Key,

                    sales = g.Sum(s => s.OrderNet)

                }).OrderByDescending(r => r.day);

            return dailyOrder;


        }
        [HttpGet]
        public object GetMonthlyRevenueForShop(DataSourceLoadOptions loadOptions)
        {
            var dailyOrder = _context.Orders
                .Where(o => (o.ShopId == _userManager.GetUserAsync(User).Result.EntityId) && (o.OrderDate.Year == DateTime.Now.Year))

                .GroupBy(c => c.OrderDate.Date.Month).

                Select(g => new
                {

                    day = g.Key,

                    sales = g.Sum(s => s.OrderNet)

                }).OrderByDescending(r => r.day);

            return dailyOrder;


        }
        [HttpGet]
        public object GetYealyOrdersCount()
        {
            var dailyOrder = _context.Orders
               .GroupBy(c => c.OrderDate.Date.Year).

               Select(g => new
               {

                   age = g.Key.ToString(),

                   number = g.Count()

               }).OrderBy(r => r.number).ThenBy(r => DateTime.Now.Year);

            return dailyOrder;
        }
        [HttpGet]
        public object GetYealyOrdersCountForShop()
        {
            var dailyOrder = _context.Orders
                .Where(o => o.ShopId == _userManager.GetUserAsync(User).Result.EntityId)
               .GroupBy(c => c.OrderDate.Date.Year).

               Select(g => new
               {

                   age = g.Key.ToString(),

                   number = g.Count()

               }).OrderBy(r => r.number).ThenBy(r => DateTime.Now.Year);

            return dailyOrder;
        }
        [HttpGet]
        public object GetYealyRevenue()
        {
            var yearlyOrder = _context.Orders
               .GroupBy(c => c.OrderDate.Date.Year).

               Select(g => new
               {

                   age = g.Key.ToString(),

                   number = g.Sum(s => s.OrderNet)

               }).OrderBy(r => r.number).ThenBy(r => DateTime.Now.Year);

            return yearlyOrder;
        }
        [HttpGet]
        public object GetYealyRevenueForShop()
        {
            var yearlyOrder = _context.Orders
                .Where(o => o.ShopId == _userManager.GetUserAsync(User).Result.EntityId)
               .GroupBy(c => c.OrderDate.Date.Year).

               Select(g => new
               {

                   age = g.Key.ToString(),

                   number = g.Sum(s => s.OrderNet)

               }).OrderBy(r => r.number).ThenBy(r => DateTime.Now.Year);

            return yearlyOrder;
        }


    }
}
