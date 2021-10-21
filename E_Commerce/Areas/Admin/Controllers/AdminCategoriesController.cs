using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Models;
using Blogs.Helpers;
using PagedList.Core;
using Microsoft.AspNetCore.Http;
using System.IO;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminCategoriesController : Controller
    {
        private readonly MarketDBContext _context;
        public INotyfService _notifyService { get; }
        public AdminCategoriesController(MarketDBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/AdminCategories
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
            var lsCategory = _context.Categories
                .OrderBy(x => x.CatId);

            PagedList<Category> models = new PagedList<Category>(lsCategory, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/AdminCategories/Create
        public IActionResult Create()
        {
            ViewData["DanhMucGoc"] = new SelectList(_context.Categories.Where(x => x.Levels == 1), "CatId", "CatName");
            return View();
        }

        // POST: Admin/AdminCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatId,CatName,Description,ParentId,Levels,Ordering,Published,Thumb,Title,Alias,MetaDesc,MetaKey,Cover,SchemaMarkup")] Category category, IFormFile fthumb, IFormFile fcover)
        {
            if (ModelState.IsValid)
            {
                category.Alias = Utilities.ToUrlFriendly(category.CatName);
                if (category.ParentId == null)
                {
                    category.Levels = 1;
                }
                else
                {
                    category.Levels = category.ParentId == 0 ? 1 : 2;
                }
                if (fthumb != null)
                {
                    string extension = Path.GetExtension(fthumb.FileName); 
                    string image = "thumb_" + Utilities.ToUrlFriendly(category.CatName) + "_preview" + extension;
                    category.Thumb = await Utilities.UploadFile(fthumb, @"categories", image.ToLower());
                }
                if (fcover != null)
                {
                    string extension = Path.GetExtension(fcover.FileName);
                    string image = "cover_" + Utilities.ToUrlFriendly(category.CatName) + extension;
                    category.Cover = await Utilities.UploadFile(fcover, @"covers", image.ToLower());
                }
                if (string.IsNullOrEmpty(category.Thumb)) category.Thumb = "default_cat_thumb.jpg";
                if (string.IsNullOrEmpty(category.Cover)) category.Cover = "default_cat_cv.jpg";

                _context.Add(category);
                _notifyService.Success("Create Success");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/AdminCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/AdminCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CatId,CatName,Description,ParentId,Levels,Ordering,Published,Thumb,Title,Alias,MetaDesc,MetaKey,Cover,SchemaMarkup")] Category category, IFormFile fthumb, IFormFile fcover)
        {
            if (id != category.CatId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    category.Alias = Utilities.ToUrlFriendly(category.CatName);
                    if (category.ParentId == null)
                    {
                        category.Levels = 1;
                    }
                    else
                    {
                        category.Levels = category.ParentId == 0 ? 1 : 2;
                    }
                    if (fthumb != null)
                    {
                        string extension = Path.GetExtension(fthumb.FileName);
                        string image = "thumb_" + Utilities.ToUrlFriendly(category.CatName) + "_preview" + extension;
                        category.Thumb = await Utilities.UploadFile(fthumb, @"categories", image.ToLower());
                    }
                    if (fcover != null)
                    {
                        string extension = Path.GetExtension(fcover.FileName);
                        string image = "cover_" + Utilities.ToUrlFriendly(category.CatName) + extension;
                        category.Cover = await Utilities.UploadFile(fcover, @"covers", image.ToLower());
                    }

                    _context.Update(category);
                    _notifyService.Success("Change Success");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CatId))
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
            return View(category);
        }

        // GET: Admin/AdminCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            _notifyService.Success("Delete Success");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CatId == id);
        }
    }
}
