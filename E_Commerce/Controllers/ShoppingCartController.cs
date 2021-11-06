using AspNetCoreHero.ToastNotification.Abstractions;
using E_Commerce.Extension;
using E_Commerce.Models;
using E_Commerce.ModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly MarketDBContext _context;
        public INotyfService _notifyService { get; }

        public ShoppingCartController(MarketDBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }
        //Khoi tao cart rong

        public List<CartItem> CART
        {
            get
            {
                var cart = HttpContext.Session.Get<List<CartItem>>("CART");
                if (cart == default(List<CartItem>))
                {
                    cart = new List<CartItem>();
                }
                return cart;
            }
        }

        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            List<int> lsProductIDs = new List<int>();
            var lsCart = CART;
            foreach(var item in lsCart)
            {
                lsProductIDs.Add(item.product.ProductId);
            }

            //Take BestSeller products
            List<Product> _lsProducts = _context.Products
                .OrderByDescending(x => x.ProductId)
                .Where(x => x.BestSellers == true && !lsProductIDs.Contains(x.ProductId))
                .Take(6)
                .ToList();
            ViewBag.lsProducts = _lsProducts;
            return View(CART);
        }
  
        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productID, int? amount)
        {
            try
            {
                List<CartItem> cart = CART;
                CartItem item = cart.SingleOrDefault(c => c.product.ProductId == productID);
                if (item != null)
                {
                    item.amount = item.amount + amount.Value;
                    HttpContext.Session.Set<List<CartItem>>("CART", cart);
                }
                else
                {
                    Product _product = _context.Products.SingleOrDefault(x => x.ProductId == productID);
                    item = new CartItem
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        product = _product
                    };
                    cart.Add(item);
                    _notifyService.Success("Thêm Sản Phẩm Thành Công");
                }
                //set cart session
                HttpContext.Session.Set<List<CartItem>>("CART", cart);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? amount)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("CART");
            try
            {
                if (cart != null)
                {
                    CartItem item = cart.SingleOrDefault(c => c.product.ProductId == productID);
                    if(item != null && amount.HasValue)
                    {
                        item.amount = amount.Value;
                    }
                    HttpContext.Session.Set<List<CartItem>>("CART", cart);
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("api/cart/remove")]
        public IActionResult RemoveCart(int productID)
        {
            try
            {
                List<CartItem> cart = CART;
                CartItem item = cart.SingleOrDefault(c => c.product.ProductId == productID);
                if (item != null)
                {
                    cart.Remove(item);
                }
                HttpContext.Session.Set<List<CartItem>>("CART", cart);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
    }
}
