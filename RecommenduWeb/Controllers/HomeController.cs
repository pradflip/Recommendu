using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecommenduWeb.Models;
using RecommenduWeb.Models.ViewModels;
using System.Diagnostics;

namespace RecommenduWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<Usuario> _signInManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<Usuario> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var userName = _signInManager.UserManager.GetUserName(User);
                return Redirect($"~/postagens/{userName}");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}