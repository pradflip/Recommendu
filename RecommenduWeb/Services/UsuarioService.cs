using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecommenduWeb.Data;
using RecommenduWeb.Models;

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
                user.Reputacao = user.Reputacao > 0 ? user.Reputacao-- : user.Reputacao = 0;
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
}
