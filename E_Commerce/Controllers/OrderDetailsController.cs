using E_Commerce.Models;
using E_Commerce.ModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly MarketDBContext _context;

        public OrderDetailsController(MarketDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            try
            {
                var accountID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(accountID)) return RedirectToAction("Login", "Accounts");
                var cs = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(accountID));
                if (cs == null) return NotFound();
                var order = _context.Orders.Include(x => x.TransactStatus).FirstOrDefault(z => z.OrderId == id && Convert.ToInt32(accountID) == z.CustomerId);
                if (order == null) return NotFound();

                var orderDetails = _context.OrderDetails.Include(x => x.Product).AsNoTracking().Where(x => x.OrderId == id).OrderBy(x => x.OrderDetailId).ToList();

                SeeDetails seedetails = new SeeDetails();
                seedetails.order = order;
                seedetails.orderDetails = orderDetails;

                return PartialView("Details", seedetails);
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
