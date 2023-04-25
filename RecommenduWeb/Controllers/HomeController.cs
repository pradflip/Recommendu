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
            try
            {
                if (_signInManager.IsSignedIn(User))
                {
                    var userName = _signInManager.UserManager.GetUserName(User);
                    if (userName == null)
                    {
                        return RedirectToAction(nameof(Error), new { mensagem = "Usuário não identificado.", isNotFound = true });
                    }

                    return RedirectToAction("Index", "Postagens", new { userName = userName });
                    //return Redirect($"~/postagens/{userName}");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { mensagem = ex.Message, isNotFound = false });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string mensagem, bool isNotFound)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Mensagem = mensagem, IsNotFound = isNotFound });
        }
    }
}