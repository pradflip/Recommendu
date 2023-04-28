
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using Microsoft.IdentityModel.Tokens;
using RecommenduWeb.Models;
using RecommenduWeb.Models.ViewModels;
using RecommenduWeb.Services;
using System.Diagnostics;
using static RecommenduWeb.AnaliseDescricao;
using static TorchSharp.torch.nn;

namespace RecommenduWeb.Controllers
{
    [Authorize]
    public class PostagensController : Controller
    {
        private readonly PostService _postService;
        private readonly LocalidadeService _localidadeService;
        private readonly ComentarioService _comentarioService;
        private readonly UserManager<Usuario> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _predictionEnginePool;

        public PostagensController(PostService postagemService, LocalidadeService localidadeService, ComentarioService comentarioService, UserManager<Usuario> userManager, IWebHostEnvironment environment, PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool)
        {
            _postService = postagemService;
            _localidadeService = localidadeService;
            _comentarioService = comentarioService;
            _userManager = userManager;
            _environment = environment;
            _predictionEnginePool = predictionEnginePool;
        }

        public async Task<IActionResult> Index(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                ViewData["UserName"] = user.UserName;
                var vm = new PostagemViewModel();
                vm.PostagemProduto = await _postService.BuscarProdutoPorUsuarioAsync(user.Id);
                vm.PostagemServico = await _postService.BuscarServicoPorUsuarioAsync(user.Id);

                return View(vm);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { mensagem = ex.Message, isNotFound = false });
            }
        }

        public async Task<ActionResult> Produtos(string? titulo, string? filtro)
        {
            try
            {
                ViewData["tituloAtual"] = titulo;
                if (titulo.IsNullOrEmpty())
                {
                    return View();
                }
                filtro = filtro == "relevantes" ? "relevantes" : "recentes";
                var user = await _userManager.GetUserAsync(User);
                var produtos = await _postService.BuscarProdutosAsync(titulo.ToLower(), filtro, user);

                if (produtos != null)
                {
                    var vm = new PostagemViewModel()
                    {
                        PostagemProduto = produtos
                    };

                    return View(vm);
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

        public async Task<ActionResult> Servicos(string? titulo, string? GetEstados, string? GetCidades, string? filtro)
        {
            try
            {
                ViewData["tituloAtual"] = titulo;
                ViewData["estadoAtual"] = GetEstados;
                ViewData["cidadeAtual"] = GetCidades;
                var estados = await _localidadeService.EstadoSelectListAsync();
                ViewBag.GetEstados = estados;
                var cidades = await _localidadeService.GetCidadesAsync(GetEstados);
                ViewBag.GetCidades = cidades;

                if (titulo.IsNullOrEmpty())
                {
                    return View();
                }

                GetEstados = GetEstados != null && GetEstados != "0" ? estados.Where(p => p.Value == GetEstados).First().Text : "";
                GetCidades = GetCidades != null && GetCidades != "0" ? cidades.Where(p => p.Value == GetCidades).First().Text : "";
                filtro = filtro == "relevantes" ? "relevantes" : "recentes";
                var user = await _userManager.GetUserAsync(User);
                var servicos = await _postService.BuscarServicosAsync(titulo.ToLower(), filtro, GetEstados.ToLower(), GetCidades.ToLower(), user);

                if (servicos != null)
                {
                    var vm = new PostagemViewModel()
                    {
                        PostagemServico = servicos
                    };

                    return View(vm);
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

        public async Task<IActionResult> Details(int id, string cat)
        {
            try
            {
                var referer = Request.Headers["Referer"].ToString();
                ViewData["Count"] = referer.Contains("/postagens/detalhes") ? Convert.ToInt32(TempData["Count"]) : 1;

                if (id != null && cat.Equals("Produto"))
                {
                    var prod = _postService.BuscarProdutosPorId(id);
                    prod.ComentariosPostagem = await _comentarioService.ListarComentariosAsync(prod.PostagemId);

                    if (prod == null)
                    {
                        return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem identificada.", isNotFound = true });
                    }

                    var vm = new PostagemViewModel
                    {
                        PostagemId = prod.PostagemId,
                        Categoria = prod.Categoria,
                        Titulo = prod.Titulo,
                        Descricao = prod.Descricao,
                        PublicoAlvo = prod.PublicoAlvo,
                        DtPostagem = prod.DtPostagem,
                        ImgPostagem = prod.ImgPostagem,
                        Fabricante = prod.Fabricante,
                        LinkProduto = prod.LinkProduto,
                        Curtidas = prod.Curtidas,
                        ComentarioPostagem = prod.ComentariosPostagem
                    };
                    ViewData["UserId"] = prod.Usuario.Id;

                    return View(vm);
                }
                else if (id != null && cat.Equals("Serviço"))
                {
                    var serv = _postService.BuscarServicosPorId(id);
                    serv.ComentariosPostagem = await _comentarioService.ListarComentariosAsync(serv.PostagemId);

                    if (serv == null)
                    {
                        return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem identificada.", isNotFound = true });
                    }

                    var vm = new PostagemViewModel
                    {
                        PostagemId = serv.PostagemId,
                        Categoria = serv.Categoria,
                        Titulo = serv.Titulo,
                        Descricao = serv.Descricao,
                        PublicoAlvo = serv.PublicoAlvo,
                        DtPostagem = serv.DtPostagem,
                        ImgPostagem = serv.ImgPostagem,
                        Estado = serv.Estado,
                        Cidade = serv.Cidade,
                        Endereco = serv.Endereco,
                        Contato = serv.Contato,
                        Curtidas = serv.Curtidas,
                        ComentarioPostagem = serv.ComentariosPostagem
                    };
                    ViewData["UserId"] = serv.Usuario.Id;

                    return View(vm);
                }
                else
                {
                    return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem identificada.", isNotFound = true });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { mensagem = ex.Message, isNotFound = false });

            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                var vm = new PostagemViewModel();
                ViewData["Estado"] = await _localidadeService.EstadoSelectListAsync();
                return View(vm);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { mensagem = ex.Message, isNotFound = false });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Categoria,Titulo,Descricao,PublicoAlvo,PostFile,Fabricante,LinkProduto,Estado,Cidade,Endereco,Contato")] PostagemViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(vm);
                }

                else
                {
                    var user = await _userManager.GetUserAsync(HttpContext.User);
                    var estados = await _localidadeService.EstadoSelectListAsync();
                    ViewData["Estado"] = estados;
                    vm.Estado = estados.Where(p => p.Value == vm.Estado).First().Text;

                    vm.Cidade = vm.Cidade == null ? null : vm.Cidade.Titleize();
                    var webRoot = _environment.WebRootPath + @"\Resources\PostImages";
                    int postId = await _postService.PublicarAsync(vm, user, webRoot);

                    var descricao = new ModelInput { Col1 = vm.Descricao };
                    var previsao = _predictionEnginePool.Predict(descricao);
                    var opiniao = previsao.PredictedLabel == 0 ? "boa" : "ruim";
                    if (opiniao.Equals("ruim"))
                    {
                        await _postService.AddReportPostagemAsync(postId, vm.Categoria);
                    }
                    return RedirectToAction("Index", "Postagens", new { userName = user.UserName });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { mensagem = ex.Message, isNotFound = false });
            }
        }

        public async Task<IActionResult> Edit(int id, string cat)
        {
            try
            {
                ViewData["Estado"] = await _localidadeService.EstadoSelectListAsync();
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (id != null && cat.Equals("Produto"))
                {
                    var prod = _postService.BuscarProdutosPorId(id);
                    if (prod != null)
                    {
                        if (user.Id == prod.Usuario.Id)
                        {
                            var vm = new PostagemViewModel
                            {
                                PostagemId = prod.PostagemId,
                                Categoria = prod.Categoria,
                                Titulo = prod.Titulo,
                                Descricao = prod.Descricao,
                                PublicoAlvo = prod.PublicoAlvo,
                                ImgPostagem = prod.ImgPostagem,
                                Fabricante = prod.Fabricante,
                                LinkProduto = prod.LinkProduto
                            };

                            return View(vm);
                        }
                        else
                        {
                            return RedirectToAction("Error", "Home", new { mensagem = "Você não possui permissão para editar essa postagem.", isNotFound = false });
                        }
                    }
                    else
                    {
                        return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem identificada.", isNotFound = true });
                    }
                }
                else if (id != null && cat.Equals("Serviço"))
                {
                    var serv = _postService.BuscarServicosPorId(id);
                    if (serv != null)
                    {
                        if (user.Id == serv.Usuario.Id)
                        {
                            var vm = new PostagemViewModel
                            {
                                PostagemId = serv.PostagemId,
                                Categoria = serv.Categoria,
                                Titulo = serv.Titulo,
                                Descricao = serv.Descricao,
                                PublicoAlvo = serv.PublicoAlvo,
                                ImgPostagem = serv.ImgPostagem,
                                Estado = serv.Estado,
                                Cidade = serv.Cidade,
                                Endereco = serv.Endereco,
                                Contato = serv.Contato
                            };

                            return View(vm);
                        }
                        else
                        {
                            return RedirectToAction("Error", "Home", new { mensagem = "Você não possui permissão para editar essa postagem.", isNotFound = false });
                        }
                    }
                    else
                    {
                        return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem identificada.", isNotFound = true });
                    }
                }
                else
                {
                    return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem ou categoria identificada.", isNotFound = true });
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { mensagem = ex.Message, isNotFound = false });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostagemId,Categoria,Titulo,Descricao,PublicoAlvo,PostFile,Fabricante,LinkProduto,Estado,Cidade,Endereco,Contato")] PostagemViewModel vm)
        {
            try
            {
                if (id != vm.PostagemId)
                {
                    return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem identificada.", isNotFound = true });
                }

                if (ModelState.IsValid)
                {
                    var webRoot = _environment.WebRootPath + @"\Resources\PostImages";
                    var user = await _userManager.GetUserAsync(HttpContext.User);

                    if (vm.Categoria.Equals("Produto"))
                    {
                        var prod = _postService.BuscarProdutosPorId(id);
                        if (prod != null)
                        {
                            if (user.Id != prod.Usuario.Id)
                            {
                                return RedirectToAction("Error", "Home", new { mensagem = "Você não possui permissão para editar essa postagem.", isNotFound = false });
                            }
                            await _postService.AtualizarPostagemAsync(vm, webRoot, prod, null);
                        }
                        else
                        {
                            return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem identificada.", isNotFound = true });
                        }
                    }
                    else if (vm.Categoria.Equals("Serviço"))
                    {
                        var estados = await _localidadeService.EstadoSelectListAsync();
                        ViewData["Estado"] = estados;
                        vm.Estado = estados.Where(p => p.Value == vm.Estado).First().Text;

                        var serv = _postService.BuscarServicosPorId(id);
                        if (serv != null)
                        {
                            if (user.Id != serv.Usuario.Id)
                            {
                                return RedirectToAction("Error", "Home", new { mensagem = "Você não possui permissão para editar essa postagem.", isNotFound = false });
                            }
                            await _postService.AtualizarPostagemAsync(vm, webRoot, null, serv);
                        }
                        else
                        {
                            return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem identificada.", isNotFound = true });
                        }
                    }

                    var descricao = new ModelInput { Col1 = vm.Descricao };
                    var previsao = _predictionEnginePool.Predict(descricao);
                    var opiniao = previsao.PredictedLabel == 0 ? "boa" : "ruim";
                    if (opiniao.Equals("ruim"))
                    {
                        await _postService.AddReportPostagemAsync(id, vm.Categoria);
                    }

                    return RedirectToAction("Index", "Postagens", new { userName = user.UserName });
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { mensagem = ex.Message, isNotFound = false });
            }
        }

        public async Task<IActionResult> Delete(int id, string cat)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (id != null && cat.Equals("Produto"))
                {
                    var prod = _postService.BuscarProdutosPorId(id);
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
                            return RedirectToAction("Error", "Home", new { mensagem = "Você não possui permissão para excluir essa postagem.", isNotFound = false });
                        }
                    }
                    else
                    {
                        return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem identificada.", isNotFound = true });
                    }
                }
                else if (id != null && cat.Equals("Serviço"))
                {
                    var serv = _postService.BuscarServicosPorId(id);
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
                            return RedirectToAction("Error", "Home", new { mensagem = "Você não possui permissão para excluir essa postagem.", isNotFound = false });
                        }
                    }
                    else
                    {
                        return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem identificada.", isNotFound = true });
                    }

                }
                else
                {
                    return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem ou categoria identificada.", isNotFound = true });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { mensagem = ex.Message, isNotFound = false });
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("PostagemId,Categoria")] PostagemViewModel vm)
        {
            try
            {
                if (vm.PostagemId == null || vm.Categoria == null)
                {
                    return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem ou categoria identificada.", isNotFound = true });
                }

                var webRoot = _environment.WebRootPath + @"\Resources\PostImages";
                var user = await _userManager.GetUserAsync(HttpContext.User);

                if (vm.Categoria.Equals("Produto"))
                {
                    var prod = _postService.BuscarProdutosPorId((int)vm.PostagemId);
                    if (prod != null)
                    {
                        if (user.Id != prod.Usuario.Id)
                        {
                            return RedirectToAction("Error", "Home", new { mensagem = "Você não possui permissão para excluir essa postagem.", isNotFound = false });
                        }
                        await _postService.DeletarPostagemAsync(prod.Categoria, webRoot, prod, null);
                    }
                    else
                    {
                        return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem identificada.", isNotFound = true });
                    }
                }
                else if (vm.Categoria.Equals("Serviço"))
                {
                    var serv = _postService.BuscarServicosPorId((int)vm.PostagemId);
                    if (serv != null)
                    {
                        if (user.Id != serv.Usuario.Id)
                        {
                            return RedirectToAction("Error", "Home", new { mensagem = "Você não possui permissão para excluir essa postagem.", isNotFound = false });
                        }
                        await _postService.DeletarPostagemAsync(serv.Categoria, webRoot, null, serv);
                    }
                    else
                    {
                        return RedirectToAction("Error", "Home", new { mensagem = "Nenhuma postagem identificada.", isNotFound = true });
                    }
                }
                //return Redirect($"~/usuarios/{user.UserName}");
                return RedirectToAction("Index", "Postagens", new { userName = user.UserName });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { mensagem = ex.Message, isNotFound = false });
            }
        }

        public async Task<IActionResult> Curtir(int postId, string cat, int acao, string userId, int Count)
        {
            try
            {
                var referer = Request.Headers["Referer"].ToString();
                ViewData["Count"] = Count + 1;
                TempData["Count"] = ViewData["Count"];

                if (cat == "Produto")
                {
                    var prod = _postService.BuscarProdutosPorId(postId);
                    if (prod != null)
                    {
                        await _postService.AtualizarCurtidasAsync(acao, userId, prod, null);
                    }
                }
                else if (cat == "Serviço")
                {
                    var serv = _postService.BuscarServicosPorId(postId);
                    if (serv != null)
                    {
                        await _postService.AtualizarCurtidasAsync(acao, userId, null, serv);
                    }
                }

                //return RedirectToAction(nameof(Index));
                return Redirect(referer);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { mensagem = ex.Message, isNotFound = false });
            }
        }

        public async Task<IActionResult> Reportar(int postId, string cat, int Count)
        {
            try
            {
                var referer = Request.Headers["Referer"].ToString();
                ViewData["Count"] = Count + 1;
                TempData["Count"] = ViewData["Count"];

                await _postService.AddReportPostagemAsync(postId, cat);

                return Redirect(referer);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new { mensagem = ex.Message, isNotFound = false });
            }
        }

    }
}