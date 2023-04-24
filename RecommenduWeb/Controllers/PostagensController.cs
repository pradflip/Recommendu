
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
using static RecommenduWeb.AnaliseDescricao;
using static TorchSharp.torch.nn;

namespace RecommenduWeb.Controllers
{
    [Authorize]
    public class PostagensController : Controller
    {
        private readonly PostService _postService;
        private readonly LocalidadeService _localidadeService;
        private readonly UserManager<Usuario> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _predictionEnginePool;

        public PostagensController(PostService postagemService, LocalidadeService localidadeService, UserManager<Usuario> userManager, IWebHostEnvironment environment, PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool)
        {
            _postService = postagemService;
            _localidadeService = localidadeService;
            _userManager = userManager;
            _environment = environment;
            _predictionEnginePool = predictionEnginePool;
        }

        // GET: Postagens
        public async Task<IActionResult> Index(string userName)
        {

            var user = await _userManager.FindByNameAsync(userName);
            var vm = new PostagemViewModel();
            vm.PostagemProduto = await _postService.BuscarProdutoPorUsuarioAsync(user.Id);
            vm.PostagemServico = await _postService.BuscarServicoPorUsuarioAsync(user.Id);

            return View(vm);
        }

        // GET: Postagens/Produtos
        public async Task<ActionResult> Produtos(string? titulo, string? filtro)
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

        // GET: Postagens/Servicos
        public async Task<ActionResult> Servicos(string? titulo, string? GetEstados, string? GetCidades, string? filtro)
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

        // GET: Postagens/Details/5
        public async Task<IActionResult> Details(int id, string cat)
        {
            var referer = Request.Headers["Referer"].ToString();
            ViewData["Count"] = referer.Contains("/postagens/detalhes") ? Convert.ToInt32(TempData["Count"]) : 1;

            if (id != null && cat.Equals("Produto"))
            {
                var prod = await _postService.BuscarProdutosPorIdAsync(id);
                if (prod == null) { return NotFound(); }

                var vm = new PostagemViewModel
                {
                    PostagemId = prod.PostagemId,
                    Categoria = prod.Categoria,
                    Titulo = prod.Titulo,
                    Descricao = prod.Descricao,
                    PublicoAlvo = prod.PublicoAlvo,
                    ImgPostagem = prod.ImgPostagem,
                    Fabricante = prod.Fabricante,
                    LinkProduto = prod.LinkProduto,
                    Curtidas = prod.Curtidas
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
                    Titulo = serv.Titulo,
                    Descricao = serv.Descricao,
                    PublicoAlvo = serv.PublicoAlvo,
                    ImgPostagem = serv.ImgPostagem,
                    Estado = serv.Estado,
                    Cidade = serv.Cidade,
                    Endereco = serv.Endereco,
                    Contato = serv.Contato,
                    Curtidas = serv.Curtidas
                };

                return View(vm);
            }
            else
            {
                return NotFound();
            }
        }

        // GET: Postagens/Create
        public async Task<IActionResult> Create()
        {
            var vm = new PostagemViewModel();
            ViewData["Estado"] = await _localidadeService.EstadoSelectListAsync();
            return View(vm);
        }

        // POST: Postagens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Categoria,Titulo,Descricao,PublicoAlvo,PostFile,Fabricante,LinkProduto,Estado,Cidade,Endereco,Contato")] PostagemViewModel vm)
        {
            if (vm.Categoria.Equals("0"))
            {
                return View(vm);
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (ModelState.IsValid)
            {
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

            }
            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Index", "Postagens", new { userName = user.UserName });
        }

        // GET: Postagens/Edit/5
        public async Task<IActionResult> Edit(int id, string cat)
        {
            ViewData["Estado"] = await _localidadeService.EstadoSelectListAsync();
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
                            Titulo = prod.Titulo,
                            Descricao = prod.Descricao,
                            PublicoAlvo = prod.PublicoAlvo,
                            ImgPostagem = prod.ImgPostagem,
                            Fabricante = prod.Fabricante,
                            LinkProduto = prod.LinkProduto
                        };

                        return View(vm);
                    }
                    else { return Unauthorized(); }
                }
                else { return NotFound(); }
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
                    else { return Unauthorized(); }
                }
                else { return NotFound(); }
            }
            else { return NotFound(); }
        }

        // POST: Postagens/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostagemId,Categoria,Titulo,Descricao,PublicoAlvo,PostFile,Fabricante,LinkProduto,Estado,Cidade,Endereco,Contato")] PostagemViewModel vm)
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
                    var estados = await _localidadeService.EstadoSelectListAsync();
                    ViewData["Estado"] = estados;
                    vm.Estado = estados.Where(p => p.Value == vm.Estado).First().Text;

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
            //return Redirect($"~/usuarios/{user.UserName}");
            return RedirectToAction("Index", "Usuarios", new { userName = user.UserName });
        }

        public async Task<IActionResult> Curtir(int postId, string cat, int acao, string userId, int Count)
        {
            var referer = Request.Headers["Referer"].ToString();
            ViewData["Count"] = Count + 1;
            TempData["Count"] = ViewData["Count"];

            if (cat == "Produto")
            {
                var prod = await _postService.BuscarProdutosPorIdAsync(postId);
                if (prod != null)
                {
                    await _postService.AtualizarCurtidasAsync(acao, userId, prod, null);
                }
            }
            else if (cat == "Serviço")
            {
                var serv = await _postService.BuscarServicosPorIdAsync(postId);
                if (serv != null)
                {
                    await _postService.AtualizarCurtidasAsync(acao, userId, null, serv);
                }
            }

            //return RedirectToAction(nameof(Index));
            return Redirect(referer);
        }
    }
}