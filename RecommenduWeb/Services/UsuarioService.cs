using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecommenduWeb.Data;
using RecommenduWeb.Models;
using RecommenduWeb.Models.ViewModels;

namespace RecommenduWeb.Services
{
    public class UsuarioService
    {
        private readonly UserManager<Usuario> _userManager;

        public UsuarioService(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<Usuario>> TodosUsuariosAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public string ListaReputacao(Usuario user)
        {
            string rep = user.Reputacao.ToString();
            return rep;
        }

        public async Task AtualizaReputacaoAsync(string id, int acao)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (acao == 0)
            {
                user.Reputacao--;
                user.Reputacao = user.Reputacao < 0 ? user.Reputacao = 0 : user.Reputacao;
                await _userManager.UpdateAsync(user);
            }
            else if (acao == 1)
            {
                user.Reputacao++;
                await _userManager.UpdateAsync(user);
            }
            else
            {
                throw new Exception("Erro ao tentar atualizar a reputação.");
            }
        }

        public async Task AtualizarFotoPerfilAsync(UsuarioViewModel vm, Usuario user, string webRoot)
        {
            string stringArquivo = await UploadFotoPerfilAsync(vm.PerfilFile, webRoot, user.ImagemPerfil);
            user.ImagemPerfil = stringArquivo;
            await _userManager.UpdateAsync(user);
        }

        public async Task<string> UploadFotoPerfilAsync(IFormFile imagem, string webRoot, string? nomeAntigo)
        {
            string nomeImagem = nomeAntigo;

            // Salva nova imagem e deleta anterior
            if (imagem != null)
            {
                string diretorio = webRoot;
                nomeImagem = Guid.NewGuid().ToString() + "-" + imagem.FileName;
                string path = Path.Combine(diretorio, nomeImagem);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    imagem.CopyTo(fileStream);
                }
                if (nomeAntigo != null && !nomeAntigo.Contains("default-profile-image-"))
                {
                    string pathDelete = Path.Combine(diretorio, nomeAntigo);
                    File.Delete(pathDelete);
                }
            }
            return nomeImagem;
        }
    }
}
