namespace RecommenduWeb.Models.ViewModels
{
    public class PostagemViewModel
    {
        public int? PostagemId { get; set; }
        public string? UsuarioId { get; set; }
        public string? UserName { get; set; }
        public IFormFile? FotoPerfil { get; set; }
        public string? Categoria { get; set; }
        public string? Descricao { get; set; }
        public string? PublicoAlvo { get; set; }
        public IFormFile? ImgPostagem { get; set; }
        public string? Modelo { get; set; }
        public string? Fabricante { get; set; }
        public string? LinkProduto { get; set; }
        public DateTime? TempoUso { get; set; }
        public string? NomeServico { get; set; }
        public string? Endereco { get; set; }
        public string? Contato { get; set; }
        public DateTime? DtPostagem { get; set; } 
        public int? Curtidas { get; set; }
        public ICollection<ComentarioPostagem>? ComentarioPostagem { get; set; } = new List<ComentarioPostagem>();
    }
    
}
