using System.ComponentModel.DataAnnotations;

namespace RecommenduWeb.Models
{
    public class ImagemPostagem
    {
        [Key]
        public int ImgId { get; set; }
        public string Imagem { get; set; }
        public Postagem Postagem { get; set; }

        public ImagemPostagem() { }

        public ImagemPostagem(int imgId, string imagem, Postagem postagem)
        {
            ImgId = imgId;
            Imagem = imagem;
            Postagem = postagem;
        }
    }
}
