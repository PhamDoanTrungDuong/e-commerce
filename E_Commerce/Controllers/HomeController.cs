using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Models;
using E_Commerce.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MarketDBContext _context;
        public INotyfService _notifyService { get; }
     
        public HomeController(ILogger<HomeController> logger, INotyfService notifyService, MarketDBContext context)
        {
            _logger = logger;
            _notifyService = notifyService;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeVM model = new HomeVM();

            var lsProduct = _context.Products
                .AsNoTracking()
                .Where(x => x.Active == true && x.HomeFlag == true)
                .ToList();

            var lsTotal = _context.Products
                .AsNoTracking()
                .Where(x => x.Active == true && x.HomeFlag == true)
                .Take(12)
                .ToList();

            List<ProductHomeVM> lsProductViews = new List<ProductHomeVM>();
            var lsCats = _context.Categories
                .AsNoTracking()
                .Where(x => x.Published == true)
                .OrderByDescending(x => x.Ordering)
                .ToList();

            foreach(var item in lsCats)
            {
                ProductHomeVM productHome = new ProductHomeVM();
                productHome.category = item;
                productHome.lsProducts = lsProduct.Where(x => x.CatId == item.CatId).Take(8).ToList();
                lsProductViews.Add(productHome);
            }

            var Posts = _context.Posts
                .AsNoTracking()
                .Where(x => x.Published == true && x.IsNewFeed == true)
                .OrderByDescending(x => x.CreateDate)
                .Take(1)
                .ToList();

            model.Products = lsProductViews;
            model.Posts = Posts;

            ViewBag.AllProduct = lsTotal;

            return View(model);
        }
        [Route("ve-chung-toi.html", Name = "About")]
        public IActionResult About()
        {
            return View();
        }
        [Route("lien-he.html", Name = "Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
