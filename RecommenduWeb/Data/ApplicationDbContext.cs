using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecommenduWeb.Models;
using System.Reflection.Metadata;

namespace RecommenduWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Postagem> Postagem { get; set; }
        public DbSet<PostagemProduto> PostagemProduto { get; set; }
        public DbSet<PostagemServico> PostagemServico { get; set; }
        public DbSet<ComentarioPostagem> comentarioPostagem { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Postagem>().UseTpcMappingStrategy();
            modelBuilder.Entity<PostagemProduto>();
            modelBuilder.Entity<PostagemServico>();
        }
    }
}