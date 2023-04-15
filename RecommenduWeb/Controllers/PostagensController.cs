using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RecommenduWeb.Data;
using RecommenduWeb.Models;
using RecommenduWeb.Models.ViewModels;
using RecommenduWeb.Services;

namespace RecommenduWeb.Controllers
{
    [Authorize]
    public class PostagensController : Controller
    {
        private readonly PostService _postService;
        private readonly UserManager<Usuario> _userManager;
        private readonly IWebHostEnvironment _environment;

        public PostagensController(PostService postagemService, UserManager<Usuario> userManager, IWebHostEnvironment environment)
        {
            _postService = postagemService;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: Postagens
        public async Task<IActionResult> Index()
        {
            var vm = new PostagemViewModel();
            vm.PostagemProduto = await _postService.BuscarTodosProdutosAsync();
            vm.PostagemServico = await _postService.BuscarTodosServicosAsync();

            return vm.PostagemProduto != null || vm.PostagemServico != null ?
                        View(vm) :
                        Problem("Entity set 'ApplicationDbContext.Postagem'  is null.");
        }

        // GET: Postagens/Details/5
        public async Task<IActionResult> Details(int id, string cat)
        {
            if (id != null && cat.Equals("Produto"))
            {
                var prod = await _postService.BuscarProdutosPorIdAsync(id);
                if (prod == null) { return NotFound(); }

                var vm = new PostagemViewModel
                {
                    PostagemId = prod.PostagemId,
                    Categoria = prod.Categoria,
                    Descricao = prod.Descricao,
                    PublicoAlvo = prod.PublicoAlvo,
                    ImgPostagem = prod.ImgPostagem,
                    Modelo = prod.Modelo,
                    Fabricante = prod.Fabricante,
                    LinkProduto = prod.LinkProduto
                };

                return View(vm);
            }
            else if (id != null && cat.Equals("Serviço"))
            {
                var serv = await _postService.BuscarServicosPorIdAsync(id);
                if (serv == null) { return NotFound(); }

                var vm = new PostagemViewModel
                {
                    PostagemId = serv.PostagemId,
                    Categoria = serv.Categoria,
                    Descricao = serv.Descricao,
                    PublicoAlvo = serv.PublicoAlvo,
                    ImgPostagem = serv.ImgPostagem,
                    NomeServico = serv.NomeServico,
                    Endereco = serv.Endereco,
                    Contato = serv.Contato
                };

                return View(vm);
            }
            else
            {
                return NotFound();
            }
        }

        // GET: Postagens/Create
        public IActionResult Create()
        {
            var vm = new PostagemViewModel();
            return View(vm);
        }

        // POST: Postagens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Categoria,Descricao,PublicoAlvo,PostFile,Modelo,Fabricante,LinkProduto,NomeServico,Endereco,Contato")] PostagemViewModel vm)
        {
            if (vm.Categoria.Equals("0"))
            {
                return View(vm);
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var webRoot = _environment.WebRootPath + @"\Resources\PostImages";
                await _postService.PublicarAsync(vm, user, webRoot);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Postagens/Edit/5
        public async Task<IActionResult> Edit(int id, string cat)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (id != null && cat.Equals("Produto"))
            {
                var prod = await _postService.BuscarProdutosPorIdAsync(id);
                if (prod != null)
                {
                    if (user.Id == prod.Usuario.Id)
                    {
                        var vm = new PostagemViewModel
                        {
                            PostagemId = prod.PostagemId,
                            Categoria = prod.Categoria,
                            Descricao = prod.Descricao,
                            PublicoAlvo = prod.PublicoAlvo,
                            ImgPostagem = prod.ImgPostagem,
                            Modelo = prod.Modelo,
                            Fabricante = prod.Fabricante,
                            LinkProduto = prod.LinkProduto
                        };

                        return View(vm);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else if (id != null && cat.Equals("Serviço"))
            {
                var serv = await _postService.BuscarServicosPorIdAsync(id);
                if (serv != null)
                {
                    if (user.Id == serv.Usuario.Id)
                    {
                        var vm = new PostagemViewModel
                        {
                            PostagemId = serv.PostagemId,
                            Categoria = serv.Categoria,
                            Descricao = serv.Descricao,
                            PublicoAlvo = serv.PublicoAlvo,
                            ImgPostagem = serv.ImgPostagem,
                            NomeServico = serv.NomeServico,
                            Endereco = serv.Endereco,
                            Contato = serv.Contato
                        };

                        return View(vm);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return NotFound();
                }

            }
            else
            {
                return NotFound();
            }
        }

        // POST: Postagens/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostagemId,Categoria,Descricao,PublicoAlvo,PostFile,Modelo,Fabricante,LinkProduto,NomeServico,Endereco,Contato")] PostagemViewModel vm)
        {
            if (id != vm.PostagemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var webRoot = _environment.WebRootPath + @"\Resources\PostImages";
                var user = await _userManager.GetUserAsync(HttpContext.User);

                if (vm.Categoria.Equals("Produto"))
                {
                    var prod = await _postService.BuscarProdutosPorIdAsync(id);
                    if (prod != null)
                    {
                        if (user.Id != prod.Usuario.Id)
                        {
                            return Unauthorized();
                        }
                        await _postService.AtualizarPostagemAsync(vm, webRoot, prod, null);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else if (vm.Categoria.Equals("Serviço"))
                {
                    var serv = await _postService.BuscarServicosPorIdAsync(id);
                    if (serv != null)
                    {
                        if (user.Id != serv.Usuario.Id)
                        {
                            return Unauthorized();
                        }
                        await _postService.AtualizarPostagemAsync(vm, webRoot, null, serv);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vm);
        }

        // GET: Postagens/Delete/5
        public async Task<IActionResult> Delete(int id, string cat)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (id != null && cat.Equals("Produto"))
            {
                var prod = await _postService.BuscarProdutosPorIdAsync(id);
                if (prod != null)
                {
                    if (user.Id == prod.Usuario.Id)
                    {
                        var vm = new PostagemViewModel
                        {
                            PostagemId = prod.PostagemId,
                            Categoria = prod.Categoria
                        };

                        return View(vm);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else if (id != null && cat.Equals("Serviço"))
            {
                var serv = await _postService.BuscarServicosPorIdAsync(id);
                if (serv != null)
                {
                    if (user.Id == serv.Usuario.Id)
                    {
                        var vm = new PostagemViewModel
                        {
                            PostagemId = serv.PostagemId,
                            Categoria = serv.Categoria
                        };

                        return View(vm);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return NotFound();
                }

            }
            else
            {
                return NotFound();
            }
        }

        // POST: Postagens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("PostagemId,Categoria")] PostagemViewModel vm)
        {
            if (vm.PostagemId == null || vm.Categoria == null)
            {
                return NotFound();
            }

            var webRoot = _environment.WebRootPath + @"\Resources\PostImages";
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (vm.Categoria.Equals("Produto"))
            {
                var prod = await _postService.BuscarProdutosPorIdAsync((int)vm.PostagemId);
                if (prod != null)
                {
                    if (user.Id != prod.Usuario.Id)
                    {
                        return Unauthorized();
                    }
                    await _postService.DeletarPostagemAsync(prod.Categoria, webRoot, prod, null);
                }
                else
                {
                    return NotFound();
                }
            }
            else if (vm.Categoria.Equals("Serviço"))
            {
                var serv = await _postService.BuscarServicosPorIdAsync((int)vm.PostagemId);
                if (serv != null)
                {
                    if (user.Id != serv.Usuario.Id)
                    {
                        return Unauthorized();
                    }
                    await _postService.DeletarPostagemAsync(serv.Categoria, webRoot, null, serv);
                }
                else
                {
                    return NotFound();
                }
            }
            return Redirect("~/Usuarios");
        }

        public async Task<IActionResult> Curtir(int postId, string cat, int acao)
        {
            if (cat == "Produto")
            {
                var prod = await _postService.BuscarProdutosPorIdAsync(postId);
                if (prod != null)
                {
                    await _postService.AtualizarCurtidasAsync(acao, prod, null);
                }
            }
            else if (cat == "Serviço")
            {
                var serv = await _postService.BuscarServicosPorIdAsync(postId);
                if (serv != null)
                {
                    await _postService.AtualizarCurtidasAsync(acao, null, serv);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
