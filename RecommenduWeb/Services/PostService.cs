using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RecommenduWeb.Data;
using RecommenduWeb.Models;
using RecommenduWeb.Models.ViewModels;
using System.Runtime.Intrinsics.X86;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RecommenduWeb.Services
{
    public class PostService
    {
        private readonly ApplicationDbContext _context;
        private readonly UsuarioService _usuarioService;

        public PostService(ApplicationDbContext context, UsuarioService usuarioService)
        {
            _context = context;
            _usuarioService = usuarioService;
        }

        public async Task<List<PostagemProduto>> BuscarProdutosAsync(string? titulo, string? filtro, Usuario usuario)
        {
            List<PostagemProduto> produtos = new List<PostagemProduto>();
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                switch (filtro)
                {
                    case "recentes":
                        var query = _context.PostagemProduto.Include(u => u.Usuario)
                                    .Where(u => u.Titulo.ToLower().Contains($"{titulo}"))
                                    .Where(u => u.Usuario != usuario);
                        produtos = await query.OrderBy(u => u.Titulo.IndexOf($"{titulo}"))
                                              .ThenBy(u => u.Titulo.Length)
                                              .ThenByDescending(u => u.DtPostagem)
                                              .ThenByDescending(u => u.Curtidas)
                                              .ToListAsync();
                        break;
                    case "relevantes":
                        query = _context.PostagemProduto.Include(u => u.Usuario)
                                    .Where(u => u.Titulo.ToLower().Contains($"{titulo}"))
                                    .Where(u => u.Usuario != usuario);
                        produtos = await query.OrderBy(u => u.Titulo.IndexOf($"{titulo}"))
                                              .ThenBy(u => u.Titulo.Length)
                                              .ThenByDescending(u => u.Curtidas)
                                              .ThenByDescending(u => u.DtPostagem)
                                              .ToListAsync();
                        break;
                }
            }
            return produtos;
        }

        public async Task<List<PostagemServico>> BuscarServicosAsync(string? titulo, string? filtro, string? estado, string? cidade, Usuario usuario)
        {
            List<PostagemServico> servicos = new List<PostagemServico>();
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                switch (filtro)
                {
                    case "recentes":
                        var query = _context.PostagemServico.Include(s => s.Usuario)
                                    .Where(s => s.Titulo.ToLower().Contains($"{titulo}"))
                                    .Where(s => s.Usuario != usuario);
                        if (estado != "")
                        {
                            query = query.Where(s => s.Estado.ToLower() == estado);
                        }
                        if (cidade != "")
                        {
                            query = query.Where(s => s.Cidade.ToLower() == cidade);
                        }
                        servicos = await query.OrderBy(s => s.Titulo.IndexOf($"{titulo}"))
                                              .ThenBy(s => s.Titulo.Length)
                                              .ThenByDescending(s => s.DtPostagem)
                                              .ThenByDescending(s => s.Curtidas)
                                              .ToListAsync();
                        break;
                    case "relevantes":
                        query = _context.PostagemServico.Include(s => s.Usuario)
                                    .Where(s => s.Titulo.ToLower().Contains($"{titulo}"))
                                    .Where(s => s.Usuario != usuario);
                        if (estado != "")
                        {
                            query = query.Where(s => s.Estado.ToLower() == estado);
                        }
                        if (cidade != "")
                        {
                            query = query.Where(s => s.Cidade.ToLower() == cidade);
                        }
                        servicos = await query.OrderBy(s => s.Titulo.IndexOf($"{titulo}"))
                                              .ThenBy(s => s.Titulo.Length)
                                              .ThenByDescending(s => s.Curtidas)
                                              .ThenByDescending(s => s.DtPostagem)
                                              .ToListAsync();
                        break;
                }
            }
            return servicos;
        }

        public PostagemProduto BuscarProdutosPorId(int postId)
        {
            var post = _context.PostagemProduto.Include(p => p.Usuario)
                                               .Where(p => p.PostagemId == postId)
                                               .FirstOrDefault();
            return post;
        }

        public PostagemServico BuscarServicosPorId(int postId)
        {
            var serv = _context.PostagemServico.Include(p => p.Usuario)
                                                     .Where(p => p.PostagemId == postId)
                                                     .FirstOrDefault();
            return serv;
        }

        public async Task<List<PostagemProduto>> BuscarProdutoPorUsuarioAsync(string userId)
        {
            return await _context.PostagemProduto.Include(p => p.Usuario).Where(u => u.Usuario.Id == userId).OrderByDescending(p => p.DtPostagem).ToListAsync();
        }

        public async Task<List<PostagemServico>> BuscarServicoPorUsuarioAsync(string userId)
        {
            return await _context.PostagemServico.Include(s => s.Usuario).Where(u => u.Usuario.Id == userId).OrderByDescending(s => s.DtPostagem).ToListAsync();
        }

        public async Task<int> PublicarAsync(PostagemViewModel vm, Usuario user, string webRoot)
        {
            string stringArquivo = await UploadImagemAsync(vm.PostFile, webRoot, null);
            switch (vm.Categoria)
            {
                case "1":
                    var produto = new PostagemProduto
                    {
                        Categoria = "Produto",
                        Titulo = vm.Titulo,
                        Descricao = vm.Descricao,
                        PublicoAlvo = vm.PublicoAlvo,
                        ImgPostagem = stringArquivo,
                        DtPostagem = DateTime.Now,
                        Usuario = user,
                        Fabricante = vm.Fabricante,
                        LinkProduto = vm.LinkProduto
                    };
                    await _context.AddAsync(produto);
                    await _context.SaveChangesAsync();
                    return produto.PostagemId;
                case "2":
                    var servico = new PostagemServico
                    {
                        Categoria = "Serviço",
                        Titulo = vm.Titulo,
                        Descricao = vm.Descricao,
                        PublicoAlvo = vm.PublicoAlvo,
                        ImgPostagem = stringArquivo,
                        Estado = vm.Estado,
                        Cidade = vm.Cidade,
                        DtPostagem = DateTime.Now,
                        Usuario = user,
                        Endereco = vm.Endereco,
                        Contato = vm.Contato
                    };
                    await _context.AddAsync(servico);
                    await _context.SaveChangesAsync();
                    return servico.PostagemId;
                default:
                    throw new Exception("Problemas ao tentar publicar!");
            }
        }

        public async Task DeletarPostagemAsync(string cat, string webRoot, PostagemProduto? prod, PostagemServico? serv)
        {
            string nomeImagem;
            if (cat.Equals("Produto") && prod != null)
            {
                nomeImagem = prod.ImgPostagem;
                _context.Remove(prod);
                DeletarImagem(webRoot, nomeImagem);
                await _context.SaveChangesAsync();

                await _usuarioService.RemoverReputacaoPorPostagem(prod.Usuario.Id, prod.Curtidas);
            }
            else if (cat.Equals("Serviço") && serv != null)
            {
                nomeImagem = serv.ImgPostagem;
                _context.Remove(serv);
                DeletarImagem(webRoot, nomeImagem);
                await _context.SaveChangesAsync();

                await _usuarioService.RemoverReputacaoPorPostagem(serv.Usuario.Id, serv.Curtidas);
            }
            else { throw new Exception("Problemas ao tentar deletar a publicação: categoria ou publicação não identificada."); }
        }

        public async Task AtualizarPostagemAsync(PostagemViewModel vm, string webRoot, PostagemProduto? prod, PostagemServico? serv)
        {
            string stringArquivo;

            switch (vm.Categoria)
            {
                case "Produto":
                    stringArquivo = await UploadImagemAsync(vm.PostFile, webRoot, prod.ImgPostagem);
                    prod.Titulo = vm.Titulo;
                    prod.Descricao = vm.Descricao;
                    prod.PublicoAlvo = vm.PublicoAlvo;
                    prod.ImgPostagem = stringArquivo;
                    prod.Fabricante = vm.Fabricante;
                    prod.LinkProduto = vm.LinkProduto;

                    _context.Update(prod);
                    await _context.SaveChangesAsync();
                    break;
                case "Serviço":
                    stringArquivo = await UploadImagemAsync(vm.PostFile, webRoot, serv.ImgPostagem);
                    serv.Titulo = vm.Titulo;
                    serv.Descricao = vm.Descricao;
                    serv.PublicoAlvo = vm.PublicoAlvo;
                    serv.ImgPostagem = stringArquivo;
                    serv.Estado = vm.Estado;
                    serv.Cidade = vm.Cidade;
                    serv.Endereco = vm.Endereco;
                    serv.Contato = vm.Contato;

                    _context.Update(serv);
                    await _context.SaveChangesAsync();
                    break;
                default:
                    throw new Exception("Problemas ao tentar publicar!");
            }
        }

        public async Task AtualizarCurtidasAsync(int acao, string userId, PostagemProduto? prod, PostagemServico? serv)
        {
            // userId = usuario que realizou acao de curtir - a acao sera registrada em RegistroCurtida
            // userAlvoId = usuario que recebe o like e aumento na reputacao

            string userAlvoId = null;
            bool realizouAcao = false;
            RegistroCurtida rc = new RegistroCurtida();

            // categoria = produto
            if (prod != null)
            {
                rc = await GetRegistroCurtidaAsync(userId, prod.PostagemId);
                // ação = dislike
                if (acao == 0)
                {
                    if (rc != null)
                    {
                        prod.Curtidas--;
                        prod.Curtidas = prod.Curtidas < 0 ? prod.Curtidas = 0 : prod.Curtidas;
                        _context.RegistroCurtida.Remove(rc);
                        realizouAcao = true;
                    }
                }
                // ação = like
                else if (acao == 1)
                {
                    if (rc == null)
                    {
                        prod.Curtidas++;
                        prod.Curtidas = prod.Curtidas < 0 ? prod.Curtidas = 1 : prod.Curtidas;

                        rc = new RegistroCurtida();
                        rc.UsuarioId = userId;
                        rc.PostagemId = prod.PostagemId;
                        rc.DtCurtida = DateTime.Now;
                        _context.RegistroCurtida.Add(rc);
                        realizouAcao = true;
                    }
                }

                userAlvoId = prod.Usuario.Id;
                _context.PostagemProduto.Update(prod);
            }
            // categoria = serviço
            else if (serv != null)
            {
                rc = await GetRegistroCurtidaAsync(userId, serv.PostagemId);
                // ação = dislike
                if (acao == 0)
                {
                    if (rc != null)
                    {
                        serv.Curtidas--;
                        serv.Curtidas = serv.Curtidas < 0 ? serv.Curtidas = 0 : serv.Curtidas;
                        _context.RegistroCurtida.Remove(rc);
                        realizouAcao = true;
                    }
                }
                // ação = like
                else if (acao == 1)
                {
                    if (rc == null)
                    {
                        serv.Curtidas++;
                        serv.Curtidas = serv.Curtidas < 0 ? serv.Curtidas = 1 : serv.Curtidas;

                        rc = new RegistroCurtida();
                        rc.UsuarioId = userId;
                        rc.PostagemId = serv.PostagemId;
                        rc.DtCurtida = DateTime.Now;
                        _context.RegistroCurtida.Add(rc);
                        realizouAcao = true;
                    }
                }

                userAlvoId = serv.Usuario.Id;
                _context.PostagemServico.Update(serv);
            }

            await _context.SaveChangesAsync();
            await _usuarioService.AtualizaReputacaoAsync(userAlvoId, acao, realizouAcao);
        }

        public async Task<string> UploadImagemAsync(IFormFile imagem, string webRoot, string? nomeAntigo)
        {
            string nomeImagem = null;
            if (imagem != null)
            {
                if (imagem.ContentType == "image/png" || imagem.ContentType == "image/jpeg")
                {
                    string diretorio = webRoot;
                    var splitExtensao = imagem.FileName.Split('.');
                    int indexExtensao = splitExtensao.Count() - 1;
                    nomeImagem = Guid.NewGuid().ToString() + "." + splitExtensao[indexExtensao];
                    string path = Path.Combine(diretorio, nomeImagem);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await imagem.CopyToAsync(fileStream);
                    }
                    if (nomeAntigo != null)
                    {
                        string pathDelete = Path.Combine(diretorio, nomeAntigo);
                        File.Delete(pathDelete);
                    }
                }
            }
            return nomeImagem;
        }

        public void DeletarImagem(string webRoot, string? nomeImagem)
        {
            if (webRoot != null && nomeImagem != null)
            {
                string diretorio = webRoot;
                string pathDelete = Path.Combine(diretorio, nomeImagem);
                File.Delete(pathDelete);
            }
            else { throw new Exception("Problemas ao tentar deletar imagem: Caminho ou arquivo não encontrado."); }
        }

        public string TempoPostagem(DateTime dt)
        {
            var diferencaDt = DateTime.Now.Subtract(dt);
            string tempo;

            if (diferencaDt.Days > 30) { tempo = dt.ToString("dd/MM/yyyy"); }
            else if (diferencaDt.Days >= 1) { tempo = $"Há {diferencaDt.Days}d"; }
            else if (diferencaDt.Hours >= 1) { tempo = $"Há {diferencaDt.Hours}h"; }
            else if (diferencaDt.Minutes >= 1) { tempo = $"Há {diferencaDt.Minutes}m"; }
            else { tempo = "Agora"; }

            return tempo;
        }

        public async Task AddReportPostagemAsync(int id, string cat)
        {
            if (id == null)
            {
                throw new Exception("Postagem não identificada.");
            }
            else
            {
                var reportExist = _context.ReportPostagemNegativas.Where(p => p.PostagemId == id && p.Categoria == cat).Count() > 0 ? true : false;

                if (!reportExist)
                {
                    Postagem post;

                    if (cat.Equals("1") || cat.Equals("Produto"))
                    {
                        post = BuscarProdutosPorId(id);
                    }
                    else if (cat.Equals("2") || cat.Equals("Serviço"))
                    {
                        post = BuscarServicosPorId(id);
                    }
                    else { throw new Exception("Categoria não identificada."); }

                    ReportPostagemNegativa report = new ReportPostagemNegativa()
                    {
                        UsuarioId = post.Usuario.Id,
                        PostagemId = post.PostagemId,
                        Categoria = post.Categoria,
                        Descricao = post.Descricao,
                        DtPostagem = post.DtPostagem
                    };

                    await _context.AddAsync(report);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<RegistroCurtida> GetRegistroCurtidaAsync(string userId, int postId)
        {
            RegistroCurtida rc = new RegistroCurtida();

            rc = await _context.RegistroCurtida.FirstOrDefaultAsync(r => r.UsuarioId == userId && r.PostagemId == postId);

            return rc;
        }

        public async Task<List<RegistroCurtida>> GetPostagensCurtidasAsync(string userId)
        {
            List<RegistroCurtida> curtidas = new List<RegistroCurtida>();

            curtidas = await _context.RegistroCurtida.Where(c => c.UsuarioId == userId).ToListAsync();

            curtidas = curtidas.OrderByDescending(c => c.DtCurtida).ToList();

            return curtidas;
        }
    }
}
