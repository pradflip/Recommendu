using Microsoft.AspNetCore.Mvc.Rendering;
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
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("SIGLA", typeof(string));

            // Realiza a consulta na API e recebe o response
            var response = await client.GetAsync("https://servicodados.ibge.gov.br/api/v1/localidades/estados");
            
            // Converte o conteudo JSON para String
            var jsonString = await response.Content.ReadAsStringAsync();
            
            // Como nao esta sendo usada nenhuma classe com propriedas, nao precisa desserializar a jsonString
            // Separa em vetores cada conjunto de informacoes dos Estados
            // A consulta busca todos os estados e nao apenas um
            string[] EstadoArray = jsonString.Split("},");
            int count = 0;

            foreach (var item in EstadoArray)
            {
                // Retira e armazena apenas a Sigla de cada item do array acima
                string[] linha = item.Split(',');

                // Primeiro elemento possui comprimento maior
                string itemId = count == 0 ? linha[0].Substring(7) : linha[0].Substring(6);
                string itemSigla = linha[1].Substring(9,2);

                // Adiciona linhas no DataTable com as siglas recebidas
                DataRow dr = dt.NewRow();
                dr["id"] = itemId;
                dr["sigla"] = itemSigla;
                dt.Rows.Add(dr);

                count++;
            }

            // Ordena o DataTable pela sigla de forma crescente
            dt.DefaultView.Sort = "sigla" + " " + "asc";
            dt = dt.DefaultView.ToTable();
            
            return dt;
        }

        public async Task<IEnumerable<SelectListItem>> EstadoSelectListAsync()
        {
            DataTable dt = new DataTable();

            // Consulta o serviço que realiza um request na API do IBGE e retorna um DataTable
            dt = await ListaEstadoAsync();

            // Adiciona cada linha (DataRow) em uma lista
            var dtItem = new List<DataRow>();
            foreach (DataRow dr in dt.Rows)
            {
                dtItem.Add(dr);
            }

            // Cria a lista do DropDown com um valor padrão
            List<SelectListItem> listaEstados = new List<SelectListItem>
            {
                new SelectListItem { Text = "Selecione...", Value = "0" }
            };

            // Pega cada item da lista extraída do DataTable e adiciona na lista do DropDown
            int count = 0;
            foreach (DataRow dr in dtItem)
            {
                listaEstados.Add(new SelectListItem { Text = dtItem[count].ItemArray[1].ToString(), Value = dtItem[count].ItemArray[0].ToString() });
                count++;
            }

            return listaEstados;
        }

    }
}
