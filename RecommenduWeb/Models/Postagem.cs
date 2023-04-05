using System.ComponentModel.DataAnnotations;

namespace RecommenduWeb.Models
{
    public abstract class Postagem
    {
        [Key]
        public int PostagemId { get; set; }
        public string Categoria { get; set; }
        public string Descricao { get; set; }
        public string PublicoAlvo { get; set; }
        public DateTime DtPostagem { get; set; }
        public int Curtidas { get; set; }
        public Usuario Usuario { get; set; }
        public ICollection<ComentarioPostagem> ComentariosPostagem { get; set; } = new List<ComentarioPostagem>();
        public ICollection<ImagemPostagem> imagensPostagem { get; set; }

        public Postagem() { }

        protected Postagem(int postagemId, string categoria, string descricao, string publicoAlvo, DateTime dtPostagem, int curtidas, Usuario usuario)
        {
            PostagemId = postagemId;
            Categoria = categoria;
            Descricao = descricao;
            PublicoAlvo = publicoAlvo;
            DtPostagem = dtPostagem;
            Curtidas = curtidas;
            Usuario = usuario;
        }

    }
}
