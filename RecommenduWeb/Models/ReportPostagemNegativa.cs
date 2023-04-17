using System.ComponentModel.DataAnnotations;

namespace RecommenduWeb.Models
{
    public class ReportPostagemNegativa
    {
        [Key]
        public int ReportId { get; set; }
        public DateTime DtReport { get; set; } = DateTime.Now;
        public string UsuarioId { get; set; }
        public int PostagemId { get; set; }
        public string Categoria { get; set; }
        public string Descricao { get; set; }
        public DateTime DtPostagem { get; set; }

        public ReportPostagemNegativa() { }

        public ReportPostagemNegativa(int reportId, DateTime dtReport, string usuarioId, int postagemId, string categoria, string descricao, DateTime dtPostagem)
        {
            ReportId = reportId;
            DtReport = dtReport;
            UsuarioId = usuarioId;
            PostagemId = postagemId;
            Categoria = categoria;
            Descricao = descricao;
            DtPostagem = dtPostagem;
        }
    }
}
