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
                        var query = from p in _context.PostagemProduto.Include(p => p.Usuario)
                                    where p.Titulo.ToLower().Contains($"{titulo}")
                                    where p.Usuario != usuario
                                    orderby p.Titulo.IndexOf($"{titulo}"),
                                            p.Titulo.Length ascending,
                                            p.DtPostagem descending,
                                            p.Curtidas descending
                                    select p;
                        produtos = await query.ToListAsync();
                        break;
                    case "relevantes":
                        query = from p in _context.PostagemProduto.Include(p => p.Usuario)
                                where p.Titulo.ToLower().Contains($"{titulo}")
                                where p.Usuario != usuario
                                orderby p.Titulo.IndexOf($"{titulo}"),
                                        p.Titulo.Length ascending,
                                        p.Curtidas descending,
                                        p.DtPostagem descending
                                select p;
                        produtos = await query.ToListAsync();
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
                        var query = from s in _context.PostagemServico.Include(s => s.Usuario)
                                    where s.Titulo.ToLower().Contains($"{titulo}")
                                    where s.Usuario != usuario
                                    where s.Estado.ToLower() == estado
                                    where s.Cidade.ToLower() == cidade
                                    orderby s.Titulo.IndexOf($"{titulo}"),
                                            s.Titulo.Length ascending,
                                            s.DtPostagem descending,
                                            s.Curtidas descending
                                    select s;
                        servicos = await query.ToListAsync();
                        break;
                    case "relevantes":
                        query = from s in _context.PostagemServico.Include(s => s.Usuario)
                                where s.Titulo.ToLower().Contains($"{titulo}")
                                where s.Usuario != usuario
                                where s.Estado.ToLower() == estado
                                where s.Cidade.ToLower() == cidade
                                orderby s.Titulo.IndexOf($"{titulo}"),
                                           s.Titulo.Length ascending,
                                           s.Curtidas descending,
                                           s.DtPostagem descending
                                select s;
                        servicos = await query.ToListAsync();
                        break;
                }
            }
            return servicos;
        }

        public async Task<PostagemProduto> BuscarProdutosPorIdAsync(int postId)
        {
            PostagemProduto postagem = null;
            var post = await _context.PostagemProduto.Include(p => p.Usuario).Where(p => p.PostagemId == postId).ToListAsync();
            foreach (var item in post)
            {
                postagem = item;
            }
            return postagem;
        }

        public async Task<PostagemServico> BuscarServicosPorIdAsync(int postId)
        {
            PostagemServico postagem = null;
            var post = await _context.PostagemServico.Include(p => p.Usuario).Where(p => p.PostagemId == postId).ToListAsync();
            foreach (var item in post)
            {
                postagem = item;
            }
            return postagem;
        }

        public async Task<List<PostagemProduto>> BuscarProdutoPorUsuarioAsync(string userId)
        {
            return await _context.PostagemProduto.Where(u => u.Usuario.Id == userId).ToListAsync();
        }

        public async Task<List<PostagemServico>> BuscarServicoPorUsuarioAsync(string userId)
        {
            return await _context.PostagemServico.Where(u => u.Usuario.Id == userId).ToListAsync();
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

        public async Task AtualizarCurtidasAsync(int acao, PostagemProduto? prod, PostagemServico? serv)
        {
            string userId = null;

            // categoria = produto
            if (prod != null)
            {
                // ação = dislike
                if (acao == 0)
                {
                    prod.Curtidas--;
                    prod.Curtidas = prod.Curtidas < 0 ? prod.Curtidas = 0 : prod.Curtidas;
                }
                // ação = like
                else if (acao == 1)
                {
                    prod.Curtidas++;
                    prod.Curtidas = prod.Curtidas < 0 ? prod.Curtidas = 0 : prod.Curtidas;
                }

                userId = prod.Usuario.Id;
                _context.PostagemProduto.Update(prod);
                await _context.SaveChangesAsync();
            }
            // categoria = serviço
            else if (serv != null)
            {
                // ação = dislike
                if (acao == 0)
                {
                    serv.Curtidas--;
                    serv.Curtidas = serv.Curtidas < 0 ? serv.Curtidas = 0 : serv.Curtidas;
                }
                // ação = like
                else if (acao == 1)
                {
                    serv.Curtidas++;
                    serv.Curtidas = serv.Curtidas < 0 ? serv.Curtidas = 0 : serv.Curtidas;
                }

                userId = serv.Usuario.Id;
                _context.PostagemServico.Update(serv);
                await _context.SaveChangesAsync();
            }

            await _usuarioService.AtualizaReputacaoAsync(userId, acao);
        }

        public async Task<string> UploadImagemAsync(IFormFile imagem, string webRoot, string? nomeAntigo)
        {
            string nomeImagem = null;
            if (imagem != null)
            {
                string diretorio = webRoot;
                nomeImagem = Guid.NewGuid().ToString() + "-" + imagem.FileName;
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

            if (diferencaDt.Days > 30) { tempo = dt.ToString("dd/MM/yyyy HH:mm:ss"); }
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
                throw new Exception("Erro ao tentar reportar.");
            }
            else
            {
                Postagem post;

                if (cat.Equals("1") || cat.Equals("Produto"))
                {
                    post = await BuscarProdutosPorIdAsync(id);
                }
                else if (cat.Equals("2") || cat.Equals("Serviço"))
                {
                    post = await BuscarServicosPorIdAsync(id);
                }
                else { throw new Exception("Erro. Categoria não identificada."); }

                ReportPostagemNegativa report = new ReportPostagemNegativa()
                {
                    UsuarioId = post.Usuario.Id,
                    PostagemId = (int)post.PostagemId,
                    Categoria = post.Categoria,
                    Descricao = post.Descricao,
                    DtPostagem = (DateTime)post.DtPostagem
                };

                await _context.AddAsync(report);
                await _context.SaveChangesAsync();
            }
        }
    }
}
