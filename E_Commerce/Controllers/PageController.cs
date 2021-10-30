using Blogs.Helpers;
using E_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.Controllers
{
    public class PageController : Controller
    {
        private readonly MarketDBContext _context;
        public PageController(MarketDBContext context)
        {
            _context = context;
        }

        // GET: /page/{Alias}
        [Route("/page/{Alias}", Name = "PageDetails")]
        public async Task<IActionResult> Details(string alias)
        {
            if (string.IsNullOrEmpty(alias)) return RedirectToAction("Index","Home");

            var page = await _context.Pages
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Alias == alias);

            var ls = _context.Posts
                .AsNoTracking()
                .Where(x => x.Published == true && x.Alias != alias)
                .Take(3)
                .OrderBy(x => x.CreateDate)
                .ToList();

            if (page == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.BaiVietLienQuan = ls;
            return View(page);
        }
    }
}
