using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace RecommenduWeb.Models.ViewModels
{
    public class PostagemViewModel
    {
        public int? PostagemId { get; set; }

        public string Categoria { get; set; }

        [Required(ErrorMessage = "Campo Descrição é obrigatório")]
        [Display(Name = "Descrição")]
        [StringLength(500, ErrorMessage ="Descrição deve conter entre 1 a 500 caracteres.", MinimumLength = 1)]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Campo Público alvo é obrigatório")]
        [Display(Name = "Público alvo")]
        [StringLength(100, ErrorMessage = "Descrição deve conter entre 1 a 100 caracteres.", MinimumLength = 1)]
        public string PublicoAlvo { get; set; }

        [Display(Name = "Imagem")]
        public string? ImgPostagem { get; set; }

        [Required(ErrorMessage = "Campo Imagem é obrigatório")]
        [Display(Name = "Imagem")]
        public IFormFile PostFile { get; set; }

        [StringLength(100, ErrorMessage = "Descrição deve conter entre 1 a 100 caracteres.", MinimumLength = 1)]
        public string? Modelo { get; set; }

        [StringLength(100, ErrorMessage = "Descrição deve conter entre 1 a 100 caracteres.", MinimumLength = 1)]
        public string? Fabricante { get; set; }

        [Display(Name = "Link do produto")]
        [StringLength(500, ErrorMessage = "Descrição deve conter entre 1 a 500 caracteres.", MinimumLength = 1)]
        public string? LinkProduto { get; set; }

        [Display(Name = "Nome do serviço")]
        [StringLength(100, ErrorMessage = "Descrição deve conter entre 1 a 100 caracteres.", MinimumLength = 1)]
        public string? NomeServico { get; set; }

        [Display(Name = "Endereço")]
        [StringLength(150, ErrorMessage = "Descrição deve conter entre 1 a 150 caracteres.", MinimumLength = 1)]
        public string? Endereco { get; set; }

        [StringLength(100, ErrorMessage = "Descrição deve conter entre 1 a 100 caracteres.", MinimumLength = 1)]
        public string? Contato { get; set; }

        [Display(Name = "Data de publicação")]
        public DateTime? DtPostagem { get; set; }

        public int? Curtidas { get; set; }

        public ICollection<PostagemProduto>? PostagemProduto { get; set; } = new List<PostagemProduto>();
        public ICollection<PostagemServico>? PostagemServico { get; set; } = new List<PostagemServico>();
        public ICollection<ComentarioPostagem>? ComentarioPostagem { get; set; } = new List<ComentarioPostagem>();
    }
}
