using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecommenduWeb.Models;

namespace RecommenduWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PostagemProduto> postagemProduto { get; set; }
        public DbSet<PostagemServico> postagemServico { get; set; }
        public DbSet<ComentarioPostagem> comentarioPostagem { get; set; }
        public DbSet<Postagem> Postagem { get; set; } = default!;
    }
}