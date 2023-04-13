namespace RecommenduWeb.Models.ViewModels
{
    public class UsuarioViewModel
    {
        public string? UsuarioId { get; set; }
        public string? UserName { get; set; }
        public string? NomeCompleto { get; set; }
        public int?  Reputacao { get; set; }
        public string? FotoPerfil { get; set; }
        public IFormFile? PerfilFile { get; set; }
        public ICollection<PostagemProduto>? PostagemProduto { get; set; } = new List<PostagemProduto>();
        public ICollection<PostagemServico>? PostagemServico { get; set; } = new List<PostagemServico>();
    }
}
