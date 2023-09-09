using Microsoft.EntityFrameworkCore;
using RecommenduWeb.Data;
using RecommenduWeb.Models;

namespace RecommenduWeb.Services
{
    public class ComentarioService
    {
        private readonly ApplicationDbContext _context;

        public ComentarioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task PublicarComentarioAsync(ComentarioPostagem cp)
        {
            await _context.ComentarioPostagem.AddAsync(cp);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<ComentarioPostagem>> ListarComentariosAsync(int postId)
        {
            var comentarios = await _context.ComentarioPostagem.Include(p => p.Postagem)
                                                               .Where(c => c.Postagem.PostagemId == postId)
                                                               .OrderByDescending(p => p.DtComentario).ToListAsync();

            return comentarios;
        }

        public async Task DeletarComentarioAsync(int comentId, string userId)
        {
            var comentario = _context.ComentarioPostagem.Include(p => p.Postagem)
                                                        .Where(c => c.ComentId == comentId)
                                                        .FirstOrDefault();
            if (userId == comentario.UsuarioId || userId == comentario.Postagem.Usuario.Id)
            {
                if (comentario != null)
                {
                    _context.ComentarioPostagem.Remove(comentario);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                throw new Exception("Você não tem permissão de deletar esse comentário.");
            }
        }
    }
}
