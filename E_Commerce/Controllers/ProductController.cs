using Blogs.Helpers;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly MarketDBContext _context;
        public ProductController(MarketDBContext context)
        {
            _context = context;
        }

        [Route("shop.html", Name = "ShopProduct")]
        public IActionResult Index(int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 21;  //Utilities.PAGE_SIZE;
                var lsProducts = _context.Products
                       .AsNoTracking();

                var total = _context.Products.Count();
                PagedList<Product> models = new PagedList<Product>(lsProducts, pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                ViewBag.Total = total;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
               
            
        }
        [Route("/{Alias}", Name = "ListProduct")]
        public IActionResult List(string alias, int page = 1)
        {
            try
            {
                var pageNumber = page;
                var pageSize = 21;// Utilities.PAGE_SIZE;
                var danhmuc = _context.Categories.AsNoTracking().SingleOrDefault(x => x.Alias == alias);
                var lsProducts = _context.Products
                       .AsNoTracking()
                       .Where(x => x.CatId == danhmuc.CatId)
                       .OrderByDescending(x => x.DateCreated);
                var total = _context.Products.Count();

                PagedList<Product> models = new PagedList<Product>(lsProducts, pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                ViewBag.CurrentCat = danhmuc;
                ViewBag.Total = total;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        [Route("/{Alias}-{id}.html", Name = "ProductDetails")]
        public IActionResult Details(int id)
        {
            try
            {
                var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);
                if (product == null)
                {
                    return RedirectToAction("Index");
                }
                var listProduct = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == product.CatId && x.ProductId != id && x.Active == true)
                    .Take(4)
                    .ToList();
                ViewBag.lsPr = listProduct;
                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Product");
            }
        }
    }
}
