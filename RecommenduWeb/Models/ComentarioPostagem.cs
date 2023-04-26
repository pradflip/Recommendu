using System.ComponentModel.DataAnnotations;

namespace RecommenduWeb.Models
{
    public class ComentarioPostagem
    {
        [Key]
        public int ComentId { get; set; }
        public int SubComentId { get; set; }
        public string Comentario { get; set; }
        public string UsuarioId { get; set; }
        public DateTime DtComentario { get; set; }
        public Postagem Postagem { get; set; }

        public ComentarioPostagem() { }

        public ComentarioPostagem(int comentId, int subComentId, string comentario, string usuarioId, DateTime dtComentario, Postagem postagem)
        {
            ComentId = comentId;
            SubComentId = subComentId;
            Comentario = comentario;
            UsuarioId = usuarioId;
            DtComentario = dtComentario;
            Postagem = postagem;
        }
    }
}
