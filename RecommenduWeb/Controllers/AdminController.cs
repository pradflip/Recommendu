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

        public AdminController(AdminService adminService, PostService postService)
        {
            _adminService = adminService;
            _postService = postService;
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

        public async Task<IActionResult> DeletarPostagem(int postId, string cat)
        {
            if (postId != 0 && cat != null)
            {
                if (cat == "Produto")
                {
                    var prod = _postService.BuscarProdutosPorId(postId);
                    await _adminService.DeletarPostAsync(prod, null);
                }
                else if (cat == "Serviço")
                {
                    var serv = _postService.BuscarServicosPorId(postId);
                    await _adminService.DeletarPostAsync(null, serv);
                }
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
    }
}
