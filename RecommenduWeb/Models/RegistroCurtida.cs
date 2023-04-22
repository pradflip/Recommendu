using System.ComponentModel.DataAnnotations;

namespace RecommenduWeb.Models
{
    public class RegistroCurtida
    {
        [Key]
        public int CurtidaId { get; set; }
        public string UsuarioId { get; set; }
        public int PostagemId { get; set; }
        public DateTime DtCurtida { get; set; }

        public RegistroCurtida() { }

        public RegistroCurtida(int curtidaId, string usuarioId, int postagemId, DateTime dtCurtida)
        {
            CurtidaId = curtidaId;
            UsuarioId = usuarioId;
            PostagemId = postagemId;
            DtCurtida = dtCurtida;
        }
    }
}
