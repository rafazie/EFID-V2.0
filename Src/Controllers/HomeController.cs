using Microsoft.AspNetCore.Mvc;
using EFID_V2.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Src.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var userIdentity = HttpContext.User.Identity;
            if (userIdentity.IsAuthenticated)
            {
                return View(userIdentity);
            }
            else
            {
                return RedirectToAction("Logout", "User");
            }
        }

    }
}