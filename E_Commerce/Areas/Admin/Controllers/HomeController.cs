using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Authorize]
    [Route("admin", Name = "AdminIndex")]
    [Area("Admin")]
    public class HomeController : Controller
    {
        public INotyfService _notifyService { get; }
        public HomeController(INotyfService notifyService)
        {
            _notifyService = notifyService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
