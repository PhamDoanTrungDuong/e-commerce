using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Blogs.Helpers;
using PagedList.Core;
using Microsoft.AspNetCore.Authorization;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminShippersController : Controller
    {
        private readonly MarketDBContext _context;
        public INotyfService _notifyService { get; }

        public AdminShippersController(MarketDBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/AdminShippers
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;
            var lsShippers = _context.Shippers
                .OrderBy(x => x.ShipperId);

            PagedList<Shipper> models = new PagedList<Shipper>(lsShippers, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminShippers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipper = await _context.Shippers
                .FirstOrDefaultAsync(m => m.ShipperId == id);
            if (shipper == null)
            {
                return NotFound();
            }

            return View(shipper);
        }

        // GET: Admin/AdminShippers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminShippers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShipperId,ShipperName,Phone,Comnapy,ShipDate")] Shipper shipper)
        {
            if (ModelState.IsValid)
            {
                shipper.ShipDate = DateTime.Now;
                _context.Add(shipper);
                _notifyService.Success("Create Success");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shipper);
        }

        // GET: Admin/AdminShippers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipper = await _context.Shippers.FindAsync(id);
            if (shipper == null)
            {
                return NotFound();
            }
            return View(shipper);
        }

        // POST: Admin/AdminShippers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShipperId,ShipperName,Phone,Comnapy,ShipDate")] Shipper shipper)
        {
            if (id != shipper.ShipperId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    shipper.ShipDate = DateTime.Now;
                    _context.Update(shipper);
                    _notifyService.Success("Changes Success");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShipperExists(shipper.ShipperId))
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
            return View(shipper);
        }

        // GET: Admin/AdminShippers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipper = await _context.Shippers
                .FirstOrDefaultAsync(m => m.ShipperId == id);
            if (shipper == null)
            {
                return NotFound();
            }

            return View(shipper);
        }

        // POST: Admin/AdminShippers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shipper = await _context.Shippers.FindAsync(id);
            _context.Shippers.Remove(shipper);
            _notifyService.Success("Delete Success");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShipperExists(int id)
        {
            return _context.Shippers.Any(e => e.ShipperId == id);
        }
    }
}
