namespace RecommenduWeb.Models
{
    public class PostagemServico : Postagem
    {
        public string Endereco { get; set; }
        public string Contato { get; set; }

        public PostagemServico() { }

        public PostagemServico(int postagemId, string categoria, string descricao, string publicoAlvo, DateTime dtPostagem, int curtidas, Usuario usuario, string endereco, string contato)
            : base(postagemId, categoria, descricao, publicoAlvo, dtPostagem, curtidas, usuario)
        {
            Endereco = endereco;
            Contato = contato;
        }
    }
}
