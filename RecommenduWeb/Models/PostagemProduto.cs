using Microsoft.Build.Framework;

namespace RecommenduWeb.Models
{
    public class PostagemProduto : Postagem
    {
        public string? Modelo { get; set; }
        public string? Fabricante { get; set; }
        public string? LinkProduto { get; set;}

        public PostagemProduto() { }

        public PostagemProduto(int postagemId, string categoria, string descricao, string publicoAlvo, string imgPostagem, DateTime dtPostagem, int curtidas, Usuario usuario, string modelo, string fabricante, string linkProduto, DateTime tempoUso)
            : base (postagemId, categoria, descricao, publicoAlvo, imgPostagem, dtPostagem, curtidas, usuario)
        {
            Modelo = modelo;
            Fabricante = fabricante;
            LinkProduto = linkProduto;
        }
    }
}
