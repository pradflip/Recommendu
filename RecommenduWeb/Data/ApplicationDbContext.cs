using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecommenduWeb.Models;
using System.Reflection.Metadata;

namespace RecommenduWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Postagem> Postagem { get; set; }
        public DbSet<PostagemProduto> PostagemProduto { get; set; }
        public DbSet<PostagemServico> PostagemServico { get; set; }
        public DbSet<ComentarioPostagem> ComentarioPostagem { get; set; }
        public DbSet<RegistroCurtida> RegistroCurtida { get; set; }
        public DbSet<ReportPostagemNegativa> ReportPostagemNegativas { get; set; }
        public DbSet<TreinoML> TreinoML { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Postagem>().UseTpcMappingStrategy();
            modelBuilder.Entity<PostagemProduto>();
            modelBuilder.Entity<PostagemServico>();
            modelBuilder.Entity<IdentityRole>().HasData( new IdentityRole { Id = "1",
                                                                            Name = "Admin",
                                                                            NormalizedName = "ADMIN" });
            modelBuilder.Entity<TreinoML>().HasKey(k => new { k.Valor, k.Texto });
        }
    }
}