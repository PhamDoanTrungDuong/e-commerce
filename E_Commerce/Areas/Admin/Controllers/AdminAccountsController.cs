using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Models;
using E_Commerce.Extension;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using E_Commerce.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminAccountsController : Controller
    {
        private readonly MarketDBContext _context;

        public INotyfService _notifyService { get; }

        public AdminAccountsController(MarketDBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/AdminAccounts
        public async Task<IActionResult> Index()
        {
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description");

            List<SelectListItem> lsStatus = new List<SelectListItem>();
            lsStatus.Add(new SelectListItem() { Text = "Active", Value = "1" });
            lsStatus.Add(new SelectListItem() { Text = "Block", Value = "0" });
            ViewData["lsStatus"] = lsStatus;

            var marketDBContext = _context.Accounts.Include(a => a.Role);
            return View(await marketDBContext.ToListAsync());
        }

        // GET: Admin/AdminAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description");
            return View(account);
        }

        // GET: Admin/AdminAccounts/Create
        public IActionResult Create()
        {
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description");
            return View();
        }

        // POST: Admin/AdminAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,Phone,Email,Password,Salt,Active,FullName,RoleId,LastLogin,CreateDate")] Account account)
        {
            if (ModelState.IsValid)
            {
                account.Salt = (account.Salt).ToMD5();
                string passnow = (account.Password.ToLower() + account.Salt.Trim()).ToMD5();
                account.Password = passnow;
                account.CreateDate = DateTime.Now;
                account.LastLogin = DateTime.Now;
               

                _context.Add(account);
                _notifyService.Success("Create Success");
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // GET: Admin/AdminAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description");
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // POST: Admin/AdminAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,Phone,Email,Password,Salt,Active,FullName,RoleId,LastLogin,CreateDate")] Account account)
        {
            if (id != account.AccountId)
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    _notifyService.Success("Changes Success");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // GET: Admin/AdminAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/AdminAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            _notifyService.Success("Delete Success");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }


        [Route("/edit-profile.html", Name = "EditProfile")]
        //[Authorize]
        [HttpGet]
        public IActionResult EditProfile()
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID == null) return RedirectToAction("Login", "AdminLogin", new { Area = "Admin" });
            var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoanID));
            if (account == null) return RedirectToAction("Login", "AdminLogin", new { Area = "Admin" });
            return View(account);
        }

        [Route("/edit-profile.html", Name = "EditProfile")]
        //[Authorize]
        [HttpPost]
        public IActionResult EditProfile(Account model)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID == null) return RedirectToAction("Login", "AdminLogin", new { Area = "Admin" });
            if (ModelState.IsValid)
            {
                var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoanID));
                try
                {
                    account.FullName = model.FullName;
                    account.Phone = model.Phone;
                    account.Email = model.Email;
                    _context.Update(account);
                    _notifyService.Success("Changes Success");
                    _context.SaveChanges();
                    return RedirectToAction("EditProfile", "AdminAccounts", new { Area = "Admin" });
                }
                catch
                {
                    return View(model);
                }
            }
            return View();
        }


        [Route("/doi-mat-khau.html", Name = "ChangePassword")]
        //[Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID == null) return RedirectToAction("Login", "AdminLogin", new { Area = "Admin" });
            var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoanID));
            if (account == null) return RedirectToAction("Login", "AdminLogin", new { Area = "Admin" });
            return View();
        }

        [Route("/doi-mat-khau.html", Name = "ChangePassword")]
        //[Authorize]
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID == null) return RedirectToAction("Login", "AdminLogin", new { Area = "Admin" });
            if (ModelState.IsValid)
            {
                var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoanID));

                if (account == null) return RedirectToAction("Login", "AdminLogin", new { Area = "Admin" });

                try
                {
                    string passnow = (model.PasswordNow.ToLower() + account.Salt.Trim()).ToMD5();
                    if (passnow == account.Password.Trim())
                    {
                        account.Password = (model.Password.ToLower() + account.Salt.Trim()).ToMD5();
                        _context.Update(account);
                        _notifyService.Success("Changes Success");
                        _context.SaveChanges();
                        return RedirectToAction("EditProfile", "AdminAccounts", new { Area = "Admin" });
                    }
                    else
                    {
                        View();
                    }

                }
                catch
                {
                    return View(model);
                }
            }
            return View();
        }



    }
}
