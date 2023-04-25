using Microsoft.AspNetCore.Authorization;
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

        public ComentariosController(PostService postService, ComentarioService comentarioService)
        {
            _postService = postService;
            _comentarioService = comentarioService;
        }

        [HttpPost]
        public async Task<IActionResult> EnviarComentario(int postId, string cat, string userName, string imgPerfil, int Count, [Bind("Comentario")] string comentario)
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
                        postagem = await _postService.BuscarProdutosPorIdAsync(postId);
                        if (postagem != null)
                        {
                            ComentarioPostagem cp = new ComentarioPostagem()
                            {
                                SubComentId = 0,
                                Comentario = comentario,
                                NomeUsuario = userName,
                                ImgPerfil = imgPerfil,
                                DtComentario = DateTime.Now,
                                Postagem = postagem
                            };
                            await _comentarioService.PublicarComentario(cp);
                        }
                    }
                    else if (cat == "Serviço")
                    {
                        postagem = await _postService.BuscarServicosPorIdAsync(postId);
                        if (postagem != null)
                        {
                            ComentarioPostagem cp = new ComentarioPostagem()
                            {
                                SubComentId = 0,
                                Comentario = comentario,
                                NomeUsuario = userName,
                                ImgPerfil = imgPerfil,
                                DtComentario = DateTime.Now,
                                Postagem = postagem
                            };
                            await _comentarioService.PublicarComentario(cp);
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

    }
}
