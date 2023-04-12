using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Postagem> PostagemPorPostagemIdAsync(int postId)
        {
            Postagem postagem = null;
            var post = await _context.Postagem.Include(p => p.Usuario).Where(p => p.PostagemId == postId).ToListAsync();
            foreach (var item in post)
            {
                postagem = item;
            }
            return postagem;
        }

        public async Task<List<Postagem>> PostagemPorUsuarioIdAsync(string userId)
        {
            return await _context.Postagem.Where(u => u.Usuario.Id == userId).ToListAsync();
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
                using(var fileStream = new FileStream(path, FileMode.Create))
                {
                    imagem.CopyTo(fileStream);
                }
            }
            return nomeImagem;
        }

        public async Task AtualizarCurtidasAsync(int postId, int acao)
        {
            var post = await PostagemPorPostagemIdAsync(postId);
            if (acao == 0)
            {
                post.Curtidas--;
                await _usuarioService.AtualizaReputacaoAsync(post.Usuario.Id, 0);
            }
            else if (acao == 1)
            {
                post.Curtidas++;
                await _usuarioService.AtualizaReputacaoAsync(post.Usuario.Id, 1);
            }
            else
            {
                throw new Exception("Erro ao atualizar curtidas");
            }
            _context.Postagem.Update(post);
            await _context.SaveChangesAsync();
        }
    }
}
