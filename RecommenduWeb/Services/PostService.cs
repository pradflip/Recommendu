using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RecommenduWeb.Data;
using RecommenduWeb.Models;
using RecommenduWeb.Models.ViewModels;
using System.Runtime.Intrinsics.X86;

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

        public async Task<List<Postagem>> TodasPostagensAsync()
        {
            return await _context.Postagem.Include(p => p.Usuario).ToListAsync();
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

        public async Task PublicarAsync(PostagemViewModel vm, Usuario user, string webRoot)
        {
            int tipoPostagem = int.Parse(vm.Categoria);
            string stringArquivo = await UploadImagensAsync(vm.ImgPostagem, webRoot);
            switch (tipoPostagem)
            {
                case 1:
                    var produto = new PostagemProduto
                    {
                        Categoria = "Produto",
                        Descricao = vm.Descricao,
                        PublicoAlvo = vm.PublicoAlvo,
                        ImgPostagem = stringArquivo,
                        DtPostagem = DateTime.Now,
                        Usuario = user,
                        Modelo = vm.Modelo,
                        Fabricante = vm.Fabricante,
                        LinkProduto = vm.LinkProduto,
                        TempoUso = vm.TempoUso
                    };
                    await _context.AddAsync(produto);
                    await _context.SaveChangesAsync();
                    break;
                case 2:
                    var servico = new PostagemServico
                    {
                        Categoria = "Serviço",
                        Descricao = vm.Descricao,
                        PublicoAlvo = vm.PublicoAlvo,
                        ImgPostagem = stringArquivo,
                        DtPostagem = DateTime.Now,
                        Usuario = user,
                        NomeServico = vm.NomeServico,
                        Endereco = vm.Endereco,
                        Contato = vm.Contato
                    };
                    await _context.AddAsync(servico);
                    await _context.SaveChangesAsync();
                    break;
                default:
                    throw new Exception("Problemas ao tentar publicar!");
            }


        }

        public async Task<string> UploadImagensAsync(IFormFile imagem, string webRoot)
        {
            string nomeImagem = null;
            if (imagem != null)
            {
                string diretorio = webRoot;
                nomeImagem = Guid.NewGuid().ToString() + "-" + imagem.FileName;
                string path = Path.Combine(diretorio, nomeImagem);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    imagem.CopyTo(fileStream);
                }
            }
            return nomeImagem;
        }

        public async Task AtualizarCurtidasAsync(int acao, PostagemProduto? prod, PostagemServico? serv)
        {
            try
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
                        prod.Curtidas--;
                        prod.Curtidas = prod.Curtidas < 0 ? prod.Curtidas = 0 : prod.Curtidas;
                    }

                    userId = prod.Usuario.Id;
                    _context.PostagemProduto.Update(prod);
                    await _context.SaveChangesAsync();
                }
                // categoria = serviço
                else if (serv != null)
                {
                    if (acao == 0)
                    {
                        serv.Curtidas--;
                        serv.Curtidas = serv.Curtidas < 0 ? serv.Curtidas = 0 : serv.Curtidas;
                    }
                    else if (acao == 1)
                    {
                        serv.Curtidas--;
                        serv.Curtidas = serv.Curtidas < 0 ? serv.Curtidas = 0 : serv.Curtidas;
                    }

                    userId = serv.Usuario.Id;
                    _context.PostagemServico.Update(serv);
                    await _context.SaveChangesAsync();
                }

                await _usuarioService.AtualizaReputacaoAsync(userId, acao);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar curtidas da postagem." + ex.Message);
            }


        }

        public string TempoPostagem(DateTime dt)
        {
            var diferencaDt = DateTime.Now.Subtract(dt);

            string tempo;
            if (diferencaDt.Minutes < 1) { tempo = "Agora"; }
            else if (diferencaDt.Hours < 1) { tempo = $"Há {diferencaDt.Minutes}m"; }
            else if (diferencaDt.Days < 1) { tempo = $"Há {diferencaDt.Hours}h"; }
            else { tempo = $"Há {diferencaDt.Days}d"; }

            return tempo;
        }
    }
}
