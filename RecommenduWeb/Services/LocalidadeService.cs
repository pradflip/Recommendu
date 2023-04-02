using Newtonsoft.Json;
using RecommenduWeb.Data;
using System.Data;

namespace RecommenduWeb.Services
{
    public class LocalidadeService
    {
        private readonly ApplicationDbContext _context;

        public LocalidadeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DataTable> ListaEstadoAsync()
        {
            // Instancia o HttpClient e DataTable adicionando a coluna necessaria
            HttpClient client = new HttpClient();
            DataTable dt = new DataTable();
            dt.Columns.Add("SIGLA", typeof(string));

            // Realiza a consulta na API e recebe o response
            var response = await client.GetAsync("https://servicodados.ibge.gov.br/api/v1/localidades/estados");
            
            // Converte o conteudo JSON para String
            var jsonString = await response.Content.ReadAsStringAsync();
            
            // Como nao esta sendo usada nenhuma classe com propriedas, nao precisa desserializar a jsonString
            // Separa em vetores cada conjunto de informacoes dos Estados
            // A consulta busca todos os estados e nao apenas um
            string[] EstadoArray = jsonString.Split("},");

            foreach (var item in EstadoArray)
            {
                // Retira e armazena apenas a Sigla de cada item do array acima
                string[] linha = item.Split(',');
                string itemSigla = linha[1].Substring(9,2);

                // Adiciona linhas no DataTable com as siglas recebidas
                DataRow dr = dt.NewRow();
                dr["sigla"] = itemSigla;
                dt.Rows.Add(dr);
            }

            // Ordena o DataTable pela sigla de forma crescente
            dt.DefaultView.Sort = "sigla" + " " + "asc";
            dt = dt.DefaultView.ToTable();
            
            return dt;
        }

    }
}
