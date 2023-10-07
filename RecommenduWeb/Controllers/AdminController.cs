using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecommenduWeb.Models;
using RecommenduWeb.Services;

namespace RecommenduWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;
        private readonly PostService _postService;
        private readonly IWebHostEnvironment _environment;

        public AdminController(AdminService adminService, PostService postService, IWebHostEnvironment environment)
        {
            _adminService = adminService;
            _postService = postService;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var lista = await _adminService.BuscarReportsAsync();

                return View(lista);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { mensagem = ex.Message, isNotFound = false });
            }
        }

        public async Task<IActionResult> EnviarReview(char valor, string texto)
        {
            if (valor != null && texto != null)
            {
                TreinoML ml = new TreinoML() { Valor = valor, Texto = texto };
                await _adminService.InserirReviewAsync(ml);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoverDaLista(int reportId)
        {
            if (reportId != 0)
            {
                await _adminService.RemoverDaListaAsync(reportId);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeletarPostagem(int reportId, int postId, string cat)
        {
            try
            {
                if (postId == null || cat == null)
                {
                    return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem ou categoria identificada.", isNotFound = true });
                }

                var webRoot = _environment.WebRootPath + @"\Resources\PostImages";

                if (cat.Equals("Produto"))
                {
                    var prod = _postService.BuscarProdutosPorId((int)postId);
                    if (prod != null)
                    {
                        await _postService.DeletarPostagemAsync(prod.Categoria, webRoot, prod, null);
                        await _adminService.RemoverDaListaAsync(reportId);
                    }
                    else
                    {
                        return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem identificada.", isNotFound = true });
                    }
                }
                else if (cat.Equals("Serviço"))
                {
                    var serv = _postService.BuscarServicosPorId((int)postId);
                    if (serv != null)
                    {
                        await _postService.DeletarPostagemAsync(serv.Categoria, webRoot, null, serv);
                        await _adminService.RemoverDaListaAsync(reportId);
                    }
                    else
                    {
                        return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem identificada.", isNotFound = true });
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { mensagem = ex.Message, isNotFound = false });
            }            
        }
    }
}
