using System.ComponentModel.DataAnnotations;

namespace RecommenduWeb.Models
{
    public abstract class Postagem
    {
        public int PostagemId { get; set; }
        public string Categoria { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string PublicoAlvo { get; set; }
        public string ImgPostagem { get; set; }
        public DateTime DtPostagem { get; set; }
        public int Curtidas { get; set; } = 0;
        public Usuario Usuario { get; set; }
        public ICollection<ComentarioPostagem> ComentariosPostagem { get; set; } = new List<ComentarioPostagem>();

        public Postagem() { }

        protected Postagem(int postagemId, string categoria, string titulo, string descricao, string publicoAlvo, string imgPostagem, DateTime dtPostagem, int curtidas, Usuario usuario)
        {
            PostagemId = postagemId;
            Categoria = categoria;
            Titulo = titulo;
            Descricao = descricao;
            PublicoAlvo = publicoAlvo;
            ImgPostagem = imgPostagem;
            DtPostagem = dtPostagem;
            Curtidas = curtidas;
            Usuario = usuario;
        }

    }
}
