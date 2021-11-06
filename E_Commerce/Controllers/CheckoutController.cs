using AspNetCoreHero.ToastNotification.Abstractions;
using Blogs.Helpers;
using E_Commerce.Extension;
using E_Commerce.Models;
using E_Commerce.ModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly MarketDBContext _context;
        public INotyfService _notifyService { get; }

        public CheckoutController(MarketDBContext context, INotyfService notifyService)
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
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("CART");
            var accountID = HttpContext.Session.GetString("CustomerId");
            ShoppingVM model = new ShoppingVM();
            if(accountID != null)
            {
                var cs = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(accountID));
                model.CustomerId = cs.CustomerId;
                model.FullName = cs.FullName;
                model.Email = cs.Email;
                model.Address = cs.Address;
                model.Phone = cs.Phone;
            }

            ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId","Name");
            ViewBag.CART = cart;
            return View(model);
        }

        [HttpPost]
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(ShoppingVM sh)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("CART");
            var accountID = HttpContext.Session.GetString("CustomerId");
            ShoppingVM model = new ShoppingVM();
            if (accountID != null)
            {
                var cs = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(accountID));
                model.CustomerId = cs.CustomerId;
                model.FullName = cs.FullName;
                model.Email = cs.Email;
                model.Address = cs.Address;
                model.Phone = cs.Phone;
                model.TinhThanh = sh.TinhThanh;
                model.QuanHuyen = sh.QuanHuyen;
                model.PhuongXa = sh.PhuongXa;

                cs.LocationId = sh.TinhThanh;
                cs.District = sh.QuanHuyen;
                cs.Ward = sh.PhuongXa;
                cs.Address = sh.Address;

                _context.Update(cs);
                _context.SaveChanges();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    Order order = new Order();
                    order.CustomerId = model.CustomerId;
                    order.Address = model.Address;
                    order.LocationId = model.TinhThanh;
                    order.District = model.QuanHuyen;
                    order.Ward = model.PhuongXa;
                    order.OrderDate = DateTime.Now;
                    order.TransactStatusId = 5;
                    order.ShipDate = DateTime.Now;
                    order.Deleted = false;
                    order.Paid = false;
                   // order.Note = Utilities.StripHtml(model.Note);
                    order.TotalMoney =Convert.ToInt32(cart.Sum(x => x.TotalMoney));

                    _context.Add(order);
                    _context.SaveChanges();

                    foreach(var item in cart)
                    {
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.OrderId = order.OrderId;
                        orderDetail.ProductId = item.product.ProductId;
                        orderDetail.Amount = item.amount;
                        orderDetail.TotalMoney = order.TotalMoney;
                        orderDetail.Price = item.product.Price;
                        orderDetail.CreateDate = DateTime.Now;
                        _context.Add(orderDetail);
                    }
                    _context.SaveChanges();

                    HttpContext.Session.Remove("CART");

                    _notifyService.Success("Đơn Hàng Đặt Thành Công");

                    return RedirectToAction("Index", "Home");
                }
            }
            catch(Exception ex)
            {
                ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");
                ViewBag.CART = cart;
                return View(model);
            }

            ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");
            ViewBag.CART = cart;
            return View(model);
        }

        
    }
}
