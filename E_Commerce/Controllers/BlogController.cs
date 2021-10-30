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
    public class BlogController : Controller
    {
        private readonly MarketDBContext _context;
       
        public BlogController(MarketDBContext context)
        {
            _context = context;
        }
        [Route("blog.html", Name="Blog")]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
            var lsPpsts = _context.Posts
                .AsNoTracking()
                .Include(x => x.Account)
                .OrderBy(x => x.PostId);

            PagedList<Post> models = new PagedList<Post>(lsPpsts, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminPosts/Details/5
        [Route("/tin-tuc/{Alias}-{id}.html", Name = "BlogDetails")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .AsNoTracking()
                .Include(p => p.Account)
                .SingleOrDefaultAsync(m => m.PostId == id);
            var ls = _context.Posts
                .AsNoTracking()
                .Where(x => x.Published == true && x.PostId != id)
                .Take(3)
                .OrderBy(x => x.CreateDate)
                .ToList();

            if (post == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.BaiVietLienQuan = ls;
            return View(post);
        }
    }
}
