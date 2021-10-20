using Blogs.Helpers;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly MarketDBContext _context;
        public SearchController(MarketDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult FindProduct(string keyword)
        {
            List<Product> ls = new List<Product>();
            if(string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListProductsSearchPartialView", null);
            }
            keyword = keyword.ToLower();
            ls = _context.Products
                    .AsNoTracking()
                    .Include(x => x.Cat)
                    .Where(x => x.ProductName.Contains(keyword))
                    .OrderBy(x => x.ProductName)
                    .Take(10)
                    .ToList();
            if(ls != null)
            {
                return PartialView("ListProductsSearchPartialView", ls);
            }
            else
            {
                return PartialView("ListProductsSearchPartialView", null);
            }

            
        }
        [HttpPost]
        public IActionResult FindCustomer(string keyword)
        {
            List<Customer> ls = new List<Customer>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListCustomersSearchPartial", null);
            }
            if (Utilities.IsInterger(keyword)){
                ls = _context.Customers
                    .AsNoTracking()
                    .Where(x => x.Phone.Contains(keyword))
                    .Take(10)
                    .OrderBy(x => x.CustomerId)
                    .ToList();
            }
            else
            {
                ls = _context.Customers
                    .AsNoTracking()
                    .Where(x => x.FullName.Contains(keyword) || x.Email.Contains(keyword))
                    .Take(10)
                    .OrderBy(x => x.CustomerId)
                    .ToList();
            }
            if (ls == null)
            {
                return PartialView("ListCustomersSearchPartial", null);
            }
            else
            {
                return PartialView("ListCustomersSearchPartial", ls);
            }

           
        }
        [HttpPost]
        public IActionResult FindOrder(string keyword)
        {
            List<Order> ls = new List<Order>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListOrdersSearchPartial", null);
            }
            if (Utilities.IsInterger(keyword))
            {
                int id = int.Parse(keyword.Trim());
                ls = _context.Orders
                    .AsNoTracking()
                    .Include(x => x.Customer)
                    .Include(x => x.TransactStatus)
                    .Where(x => x.Customer.Phone.Contains(keyword))
                    .Take(10)
                    .OrderBy(x => x.OrderDate)
                    .ToList();
            }
            else
            {
                ls = _context.Orders
                   .AsNoTracking()
                   .Include(x => x.Customer)
                   .Include(x => x.TransactStatus)
                   .Where(x => x.Customer.FullName.Contains(keyword) || x.Customer.Email.Contains(keyword))
                   .Take(10)
                   .OrderBy(x => x.OrderDate)
                   .ToList();
            }
            if (ls == null)
            {
                return PartialView("ListOrdersSearchPartial", null);
            }
            else
            {
                return PartialView("ListOrdersSearchPartial", ls);
            }
        }
    }
}
