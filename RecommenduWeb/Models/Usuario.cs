﻿using Microsoft.AspNetCore.Identity;

namespace RecommenduWeb.Models
{
    public class Usuario : IdentityUser
    {
        public string NomeCompleto { get; set; }
        public string? ImagemPerfil { get; set; }
        public int Reputacao { get; set; } = 0;
        public ICollection<Postagem> Postagens { get; set; } = new List<Postagem>();

        public Usuario() { }

        public Usuario(string nomeCompleto, string imagemPerfil, int reputacao)
        {
            NomeCompleto = nomeCompleto;
            ImagemPerfil = imagemPerfil;
            Reputacao = reputacao;
        }
    }

}