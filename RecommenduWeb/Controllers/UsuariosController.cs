using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecommenduWeb.Models;
using RecommenduWeb.Models.ViewModels;
using RecommenduWeb.Services;

namespace RecommenduWeb.Controllers
{
    [Authorize]
    [Route("Usuarios")]
    public class UsuariosController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly PostService _postService;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public UsuariosController(UsuarioService usuarioService, PostService postService, UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _usuarioService = usuarioService;
            _postService = postService;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        //[Route("/Usuarios/")] - Verificar como pegar nome do usuario
        public async Task<IActionResult> Index()
        {
            //var user = await _userManager.GetUserAsync(HttpContext.User);
            var user = await _signInManager.UserManager.GetUserAsync(User);
            var foto = user.ImagemPerfil;
            ViewData["Foto"] = foto;
            ViewData["Usuario"] = user.UserName;
            ViewData["Reputacao"] = _usuarioService.ListaReputacao(user);

            var listaPost = await _postService.PostagemPorUsuarioIdAsync(user.Id);
            
            return View(listaPost);
        }
    }
}
