using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ProjetoIntegradorVendas.Classes;

namespace ProjetoIntegradorVendas.Services
{
    public class CepService
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<Endereco> BuscarEnderecoPorCep(string cep)
        {
            try
            {
                string url = $"https://viacep.com.br/ws/{cep}/json/";

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    Endereco endereco = JsonSerializer.Deserialize<Endereco>(responseBody, options);

                    return endereco;
                }
                else
                {
                    Console.WriteLine("Erro ao consultar o CEP: " + response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocorreu uma exceção: " + ex.Message);
                return null;
            }
        }
    }
}
