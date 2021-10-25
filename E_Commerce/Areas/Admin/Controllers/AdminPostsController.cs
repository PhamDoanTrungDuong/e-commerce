using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Models;
using PagedList.Core;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using System.IO;
using Blogs.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AdminPostsController : Controller
    {
        private readonly MarketDBContext _context;
        public INotyfService _notifyService { get; }
        public AdminPostsController(MarketDBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/AdminPosts
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
            var lsPpsts = _context.Posts
                .Include(x => x.Account)
                .OrderBy(x => x.PostId);

            PagedList<Post> models = new PagedList<Post>(lsPpsts, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Admin/AdminPosts/Create
        public IActionResult Create()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "FullName");
            return View();
        }

        // POST: Admin/AdminPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Scontents,Contents,Thumb,Published,Alias,CreateDate,Author,AccountId,Tags,CatId,IsHot,IsNewFeed,MetaDesc,MetaKey,Views")] Post post, IFormFile fthumb)
        {
            if (ModelState.IsValid)
            {
                post.Title = Utilities.ToTitleCase(post.Title);
                if (fthumb != null)
                {
                    string extension = Path.GetExtension(fthumb.FileName);
                    string image = "thumb_" + Utilities.ToUrlFriendly(post.Title) + "_preview" + extension;
                    post.Thumb = await Utilities.UploadFile(fthumb, @"posts", image.ToLower());
                }
                if (string.IsNullOrEmpty(post.Thumb)) post.Thumb = "default.jpg";
                post.Alias = Utilities.ToUrlFriendly(post.Title);
                post.CreateDate = DateTime.Now;

                _context.Add(post);
                await _context.SaveChangesAsync();
                _notifyService.Success("Create Success");
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", post.AccountId);
            return View(post);
        }

        // GET: Admin/AdminPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", post.AccountId);
            return View(post);
        }

        // POST: Admin/AdminPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Scontents,Contents,Thumb,Published,Alias,CreateDate,Author,AccountId,Tags,CatId,IsHot,IsNewFeed,MetaDesc,MetaKey,Views")] Post post, IFormFile fthumb)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    post.Title = Utilities.ToTitleCase(post.Title);
                    if (fthumb != null)
                    {
                        string extension = Path.GetExtension(fthumb.FileName);
                        string image = "thumb_" + Utilities.ToUrlFriendly(post.Title) + "_preview" + extension;
                        post.Thumb = await Utilities.UploadFile(fthumb, @"posts", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(post.Thumb)) post.Thumb = "default.jpg";
                    post.Alias = Utilities.ToUrlFriendly(post.Title);
                    post.CreateDate = DateTime.Now;

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Change Success");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", post.AccountId);
            return View(post);
        }

        // GET: Admin/AdminPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/AdminPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            _notifyService.Success("Delete Success");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
