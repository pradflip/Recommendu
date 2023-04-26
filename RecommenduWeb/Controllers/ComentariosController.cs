using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecommenduWeb.Models;
using RecommenduWeb.Models.ViewModels;
using RecommenduWeb.Services;
using System.Diagnostics;

namespace RecommenduWeb.Controllers
{
    [Authorize]
    public class ComentariosController : Controller
    {
        private readonly PostService _postService;
        private readonly ComentarioService _comentarioService;
        private readonly UserManager<Usuario> _userManager;

        public ComentariosController(PostService postService, ComentarioService comentarioService, UserManager<Usuario> userManager)
        {
            _postService = postService;
            _comentarioService = comentarioService;
            _userManager = userManager;
        }

        //[HttpPost]
        public async Task<IActionResult> EnviarComentario(int postId, string cat, string userId, int Count, [Bind("Comentario")] string comentario)
        {
            try
            {
                var referer = Request.Headers["Referer"].ToString();
                ViewData["Count"] = Count + 1;
                TempData["Count"] = ViewData["Count"];

                if (postId == 0 || postId == null)
                {
                    return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem identificada.", isNotFound = true });
                }

                if (ModelState.IsValid)
                {
                    Postagem postagem;

                    if (cat == "Produto")
                    {
                        postagem = _postService.BuscarProdutosPorIdAsync(postId);
                        if (postagem != null)
                        {
                            ComentarioPostagem cp = new ComentarioPostagem()
                            {
                                SubComentId = 0,
                                Comentario = comentario,
                                UsuarioId = userId,
                                DtComentario = DateTime.Now,
                                Postagem = postagem
                            };
                            await _comentarioService.PublicarComentarioAsync(cp);
                        }
                    }
                    else if (cat == "Serviço")
                    {
                        postagem = _postService.BuscarServicosPorIdAsync(postId);
                        if (postagem != null)
                        {
                            ComentarioPostagem cp = new ComentarioPostagem()
                            {
                                SubComentId = 0,
                                Comentario = comentario,
                                UsuarioId = userId,
                                DtComentario = DateTime.Now,
                                Postagem = postagem
                            };
                            await _comentarioService.PublicarComentarioAsync(cp);
                        }
                    }
                    else
                    {
                        return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma categoria identificada.", isNotFound = true });
                    }

                }

                return Redirect(referer);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { mensagem = ex.Message, isNotFound = false });
            }
        }

        public async Task<IActionResult> ExcluirComentario(int comentId, int Count)
        {
            try
            {
                var referer = Request.Headers["Referer"].ToString();
                ViewData["Count"] = Count + 1;
                TempData["Count"] = ViewData["Count"];

                if (comentId == 0 || comentId == null)
                {
                    return RedirectToAction("Error", "Home", new { mensagem = "Nenhum comentário identificada.", isNotFound = true });
                }

                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    await _comentarioService.DeletarComentarioAsync(comentId, user.Id);
                }

                return Redirect(referer);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { mensagem = ex.Message, isNotFound = false });
            }
        }
    }
}
