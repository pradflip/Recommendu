namespace RecommenduWeb.Models
{
    public class PostagemProduto : Postagem
    {
        public string Modelo { get; set; }
        public string Fabricante { get; set; }
        public string LinkProduto { get; set;}
        public DateTime TempoUso { get; set; }

        public PostagemProduto() { }

        public PostagemProduto(int postagemId, string categoria, string descricao, string publicoAlvo, DateTime dtPostagem, int curtidas, Usuario usuario, string modelo, string fabricante, string linkProduto, DateTime tempoUso)
            : base (postagemId, categoria, descricao, publicoAlvo, dtPostagem, curtidas, usuario)
        {
            Modelo = modelo;
            Fabricante = fabricante;
            LinkProduto = linkProduto;
            TempoUso = tempoUso;
        }
    }
}
