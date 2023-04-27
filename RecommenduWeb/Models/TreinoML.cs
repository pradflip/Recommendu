namespace RecommenduWeb.Models
{
    public class TreinoML
    {
        public char Valor { get; set; }
        public string Texto { get; set;}

        public TreinoML() { }

        public TreinoML(char valor, string texto)
        {
            Valor = valor;
            Texto = texto;
        }
    }
}
