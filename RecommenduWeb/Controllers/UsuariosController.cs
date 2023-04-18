using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecommenduWeb.Models;
using RecommenduWeb.Models.ViewModels;
using RecommenduWeb.Services;

namespace RecommenduWeb.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly PostService _postService;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IWebHostEnvironment _environment;

        public UsuariosController(UsuarioService usuarioService, PostService postService, UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IWebHostEnvironment environment)
        {
            _usuarioService = usuarioService;
            _postService = postService;
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var listaProd = await _postService.BuscarProdutoPorUsuarioAsync(user.Id);
            var listaServ = await _postService.BuscarServicoPorUsuarioAsync(user.Id);
            var vm = new UsuarioViewModel()
            {
                UsuarioId = user.Id,
                UserName = user.UserName,
                NomeCompleto = user.NomeCompleto,
                Reputacao = user.Reputacao,
                FotoPerfil = user.ImagemPerfil,
                PostagemProduto = listaProd,
                PostagemServico = listaServ
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Usuarios/AtualizarFoto")]
        public async Task<IActionResult> AtualizarFoto([Bind("PerfilFile")] UsuarioViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var webRoot = _environment.WebRootPath + @"\Resources\ProfileImages";
                await _usuarioService.AtualizarFotoPerfilAsync(vm, user, webRoot);

                return RedirectToAction("Index", new { userName = user.UserName });
            }
            return View(vm);
        }

        [Route("Usuarios/DeletarFoto")]
        public async Task<IActionResult> DeletarFoto()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var webRoot = _environment.WebRootPath + @"\Resources\ProfileImages";
            await _usuarioService.DeletarFotoPerfilAsync(user, webRoot);

            return RedirectToAction("Index", new {userName = user.UserName});
        }
    }
}
