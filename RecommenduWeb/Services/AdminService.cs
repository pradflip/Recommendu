using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecommenduWeb.Data;
using RecommenduWeb.Models;

namespace RecommenduWeb.Services
{
    public class AdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<List<ReportPostagemNegativa>> BuscarReportsAsync()
        {
            var lista = await _context.ReportPostagemNegativas.OrderBy(r => r.DtReport)
                                                              .ToListAsync();

            return lista;
        }

        public async Task InserirReviewAsync(TreinoML ml)
        {
            if (ml == null) throw new ArgumentNullException(nameof(ml));

            await _context.TreinoML.AddAsync(ml);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarPostAsync(PostagemProduto? prod, PostagemServico? serv)
        {
            if (prod != null && serv == null)
            {
                _context.PostagemProduto.Remove(prod);
                await _context.SaveChangesAsync();
            }
            else if (prod == null && serv != null)
            {
                _context.PostagemServico.Remove(serv);
                await _context.SaveChangesAsync();
            }
            else { throw new Exception("Falha ao tentar deletar. Argumentos passados são nulos."); }
        }

        public async Task RemoverDaListaAsync(int reportId)
        {
            var linhaReport = await _context.ReportPostagemNegativas.Where(r => r.ReportId == reportId)
                                                                    .FirstOrDefaultAsync();
            _context.ReportPostagemNegativas.Remove(linhaReport);
            await _context.SaveChangesAsync();
        }
    }
}
