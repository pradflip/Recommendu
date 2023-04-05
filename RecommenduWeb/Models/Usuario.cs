using Microsoft.AspNetCore.Identity;

namespace RecommenduWeb.Models
{
    public class Usuario : IdentityUser
    {
        public string NomeCompleto { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set;}
        public string? ImgPerfil { get; set; }
        public int Reputacao { get; set; } = 0;
        public ICollection<Postagem> Postagens { get; set; } = new List<Postagem>();
    }
}
