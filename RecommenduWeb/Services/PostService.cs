using Microsoft.AspNetCore.Identity;
using RecommenduWeb.Data;
using RecommenduWeb.Models;
using RecommenduWeb.Models.ViewModels;
using System.Runtime.Intrinsics.X86;

namespace RecommenduWeb.Services
{
    public class PostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
