using Microsoft.Build.Framework;

namespace RecommenduWeb.Models
{
    public class PostagemProduto : Postagem
    {
        public string? Fabricante { get; set; }
        public string? LinkProduto { get; set;}

        public PostagemProduto() { }

        public PostagemProduto(int postagemId, string categoria, string titulo, string descricao, string publicoAlvo, string imgPostagem, DateTime dtPostagem, int curtidas, Usuario usuario, string fabricante, string linkProduto)
            : base (postagemId, categoria, titulo, descricao, publicoAlvo, imgPostagem, dtPostagem, curtidas, usuario)
        {
            Fabricante = fabricante;
            LinkProduto = linkProduto;
        }
    }
}
