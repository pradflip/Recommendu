using Microsoft.Build.Framework;

namespace RecommenduWeb.Models
{
    public class PostagemServico : Postagem
    {
        public string? NomeServico { get; set; }
        public string? Endereco { get; set; }
        public string? Contato { get; set; }

        public PostagemServico() { }

        public PostagemServico(int postagemId, string categoria, string descricao, string publicoAlvo, string imgPostagem, DateTime dtPostagem, int curtidas, Usuario usuario, string nomeServico, string endereco, string contato)
            : base (postagemId, categoria, descricao, publicoAlvo, imgPostagem, dtPostagem, curtidas, usuario)
        {
            NomeServico = nomeServico;
            Endereco = endereco;
            Contato = contato;
        }
    }
}
