﻿using E_Commerce.Extension;
using E_Commerce.Areas.Admin.Models;
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

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminLoginController : Controller
    {

        private readonly MarketDBContext _context;

        public AdminLoginController(MarketDBContext context)
        {
            _context = context;
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("/dang-nhap.html", Name = "Login")]
        public IActionResult Login(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID != null) return RedirectToAction("Index", "Home", new { Area = "Admin" });
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/dang-nhap.html", Name = "Login")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Account kh = _context.Accounts
                        .Include(p => p.Role)
                        .SingleOrDefault(p => p.Email.ToLower() == model.Email.ToLower().Trim());

                    if (kh == null)
                    {
                        ViewBag.Error = "Thông tin đăng nhập không chính xác";
                        return View(model);
                    }
                    string pass = (model.Password.ToLower() + kh.Salt.Trim()).ToMD5();
                    if (kh.Password.Trim() != pass)
                    {
                        ViewBag.Error = "Thông tin đăng nhập không chính xác";
                        return View(model);
                    }

                    //Đăng nhập thành công
                    kh.LastLogin = DateTime.Now;
                    _context.Update(kh);
                    await _context.SaveChangesAsync();

                    var taikhoanID = HttpContext.Session.GetString("AccountId");

                    HttpContext.Session.SetString("AccountId", kh.AccountId.ToString());

                    var userClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, kh.FullName),
                        new Claim(ClaimTypes.Email, kh.Email),
                        new Claim("AccountId", kh.AccountId.ToString()),
                        new Claim("RoleId", kh.RoleId.ToString()),
                        new Claim(ClaimTypes.Role, kh.Role.RoleName)
                    };

                    var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");
                    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });
                    await HttpContext.SignInAsync(userPrincipal);

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home", new { Area = "Admin" });
                }

            }
            catch
            {
                return RedirectToAction("Login", "Home", new { Area = "Admin" });
            }
            return RedirectToAction("Login", "Home", new { Area = "Admin" });

        }

        [Route("/dang-xuat.html", Name = "Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync();
                HttpContext.Session.Remove("AccountId");
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
