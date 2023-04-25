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

        public async Task PublicarComentario(ComentarioPostagem cp)
        {
            await _context.ComentarioPostagem.AddAsync(cp);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<ComentarioPostagem>> ListarComentarios(int postId)
        {
            var comentarios = await _context.ComentarioPostagem.Include(p => p.Postagem)
                                                               .Where(c => c.Postagem.PostagemId == postId)
                                                               .OrderByDescending(p => p.DtComentario).ToListAsync();

            return comentarios;
        }
    }
}
