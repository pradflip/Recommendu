using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
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

        public async Task<List<Usuario>> BuscarUsuariosAsync(string nomeUsuario, Usuario usuario)
        {
            var query = from u in _userManager.Users
                        where u.UserName.Contains($"{nomeUsuario.ToLower()}")
                        where u != usuario
                        orderby u.UserName.IndexOf($"{nomeUsuario.ToLower()}"),
                                u.UserName.Length ascending,
                                u.Reputacao
                        select u;

            return await query.ToListAsync();
        }

        public Usuario BuscarUsuarioPorId(string userId)
        {
            var user = _userManager.Users.Where(u => u.Id == userId)
                                         .FirstOrDefault();

            return user;
        }

        public string ListaReputacao(Usuario user)
        {
            string rep = user.Reputacao.ToString();
            return rep;
        }

        public async Task AtualizaReputacaoAsync(string id, int acao, bool realizouAcao)
        {
            if (realizouAcao)
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
        }

        public async Task RemoverReputacaoPorPostagem(string userId, int likes)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.Reputacao -= likes;
                await _userManager.UpdateAsync(user);
            }
            else { throw new Exception("Erro ao tentar atualizar reputação."); }

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

        public async Task DeletarFotoPerfilAsync(Usuario user, string webRoot)
        {
            string stringArquivo = DeletarFotoDoDiretorio(webRoot, user.ImagemPerfil);
            user.ImagemPerfil = stringArquivo;
            await _userManager.UpdateAsync(user);
        }

        public string DeletarFotoDoDiretorio(string diretorio, string? nomeImagem)
        {

            if (diretorio != null)
            {
                Random random = new Random();

                if (nomeImagem != null && !nomeImagem.Contains("default-profile-image-"))
                {
                    string pathDelete = Path.Combine(diretorio, nomeImagem);
                    File.Delete(pathDelete);
                }
                int num = random.Next(1, 8);
                string imagemPadrao = $"default-profile-image-{num}.png";

                return imagemPadrao;
            }
            else { throw new Exception("Problemas ao tentar deletar imagem: Caminho ou arquivo não encontrado."); }
        }

        public async Task<List<Usuario>> BuscarTopCem()
        {
            var top = await _userManager.Users.OrderByDescending(u => u.Reputacao)
                                        .Take(100)
                                        .ToListAsync();

            return top;
        }
    }
}