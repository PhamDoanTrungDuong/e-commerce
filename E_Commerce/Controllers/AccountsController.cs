using E_Commerce.Extension;
using E_Commerce.ModelViews;
using E_Commerce.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blogs.Helpers;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace E_Commerce.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly MarketDBContext _context;
        public INotyfService _notifyService { get; }

        public AccountsController(MarketDBContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        [Route("/tai-khoan-cua-toi.html", Name = "tai-khoan")]
        public IActionResult Dashboard()
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                var cs = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                if(cs != null)
                {
                    var lsOrder = _context.Orders
                        .AsNoTracking()
                        .Include(x => x.TransactStatus)
                        .Include(x => x.OrderDetails)
                        .Where(x => x.CustomerId == cs.CustomerId)
                        .OrderByDescending(x => x.OrderDate)
                        .ToList();
                    ViewBag.lsOrders = lsOrder;
                    return View(cs);
                }
            }
            return RedirectToAction("LoginCustomer");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/dang-ky.html", Name = "dang-ky")]
        public IActionResult RegisterCustomer()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/dang-ky.html", Name = "dang-ky")]
        public async Task<IActionResult> RegisterCustomer(RegisterVM customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string salt = Utilities.GetRandomKey();

                    Customer cs = new Customer
                    {
                        FullName = customer.FullName,
                        Phone = customer.Phone.Trim().ToLower(),
                        Email = customer.Email.Trim().ToLower(),
                        Password = (customer.Password + salt.Trim()).ToMD5(),
                        Active = true,
                        Salt = salt.Trim(),
                        CreateDate = DateTime.Now
                    };

                    try
                    {
                        _context.Add(cs);
                        await _context.SaveChangesAsync();

                        //Set Session
                        HttpContext.Session.SetString("CustomerId", cs.CustomerId.ToString());
                        var taikhoanID = HttpContext.Session.GetString("CustomerId");

                        var userClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, cs.FullName),
                            new Claim(ClaimTypes.Email, cs.Email),
                            new Claim("CustomerId", cs.CustomerId.ToString()),
                        };

                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(userClaims, "User Identity");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);

                        return RedirectToAction("Dashboard", "Accounts");
                    }
                    catch
                    {
                        return RedirectToAction("RegisterCustomer", "Accounts");
                    }
                }
                else
                {
                    return View(customer);
                }
            }
            catch
            {
                return View(customer);
            }
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("/dang-nhap-khach-hang.html", Name = "dang-nhap")]
        public IActionResult LoginCustomer(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null) return RedirectToAction("Dashboard", "Accounts");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("/dang-nhap-khach-hang.html", Name = "dang-nhap")]
        public async Task<IActionResult> LoginCustomer(LoginViewModel customer, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    bool isEmail = Utilities.IsValidEmail(customer.UserName);
                    if (!isEmail) return View(customer);

                    var cs = _context.Customers
                            .AsNoTracking()
                            .SingleOrDefault(x => x.Email == customer.UserName);

                    if (cs == null) return RedirectToAction("RegisterCustomer");

                    string pass = (customer.Password + cs.Salt.Trim()).ToMD5();

                    if (cs.Password != pass)
                    {
                        ViewBag.Error = "Thông tin đăng nhập không chính xác";
                        return View(customer);
                    }

                    if (cs.Active == false)
                    {
                        return RedirectToAction("RegisterCustomer");
                    }

                    //Đăng nhập thành công
                    cs.LastLogin = DateTime.Now;
                    _context.Update(cs);
                    _notifyService.Success("Login Success");
                    await _context.SaveChangesAsync();

                    //Session
                    HttpContext.Session.SetString("CustomerId", cs.CustomerId.ToString());
                    var taikhoanID = HttpContext.Session.GetString("CustomerId");

                    //Identity
                    var userClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, cs.FullName),
                        new Claim(ClaimTypes.Email, cs.Email),
                        new Claim("CustomerId", cs.CustomerId.ToString()),
                    };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(userClaims, "User Identity");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);

                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("RegisterCustomer", "Accounts");
            }
            return View(customer);
        }

        [Route("/dang-xuat-khach-hang.html", Name = "dang-xuat")]
        public async Task<IActionResult> LogoutCustomer()
        {
            try
            {
                await HttpContext.SignOutAsync();
                HttpContext.Session.Remove("CustomerId");
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //[Authorize]
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap-khach-hang.html");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID == null) return RedirectToAction("LoginCustomer","Accounts");
            if (ModelState.IsValid)
            {
                var account = _context.Customers.AsNoTracking().FirstOrDefault(x => x.CustomerId == int.Parse(taikhoanID));

                if (account == null) return RedirectToAction("LoginCustomer", "Accounts");

                try
                {
                    string passnow = (model.PasswordNow + account.Salt.Trim()).ToMD5();
                    if (passnow == account.Password)
                    {
                        account.Password = (model.Password + account.Salt.Trim()).ToMD5();
                        _context.Update(account);
                        _notifyService.Success("Changes Success");
                        _context.SaveChanges();
                        return RedirectToAction("Dashboard", "Accounts");
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


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string phone)
        {
            try
            {
                var cs = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.Phone.ToLower() == phone.ToLower());
                if (cs != null)
                    return Json(data: "Số Điện Thoại: " + phone + " Đã được sử dụng");
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string email)
        {
            try
            {
                var cs = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.Email.ToLower() == email.ToLower());
                if (cs != null)
                    return Json(data: "Email: " + email + " Đã được sử dụng");
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }


    }
}
