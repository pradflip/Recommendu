using Microsoft.Build.Framework;

namespace RecommenduWeb.Models
{
    public class PostagemServico : Postagem
    {
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Endereco { get; set; }
        public string? Contato { get; set; }

        public PostagemServico() { }

        public PostagemServico(int postagemId, string categoria, string titulo, string descricao, string publicoAlvo, string imgPostagem, DateTime dtPostagem, int curtidas, Usuario usuario, string cidade, string estado, string endereco, string contato)
            : base (postagemId, categoria, titulo, descricao, publicoAlvo, imgPostagem, dtPostagem, curtidas, usuario)
        {
            Cidade = cidade;
            Estado = estado;
            Endereco = endereco;
            Contato = contato;
        }
    }
}
