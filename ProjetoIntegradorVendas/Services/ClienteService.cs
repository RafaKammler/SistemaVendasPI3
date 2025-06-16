using MySql.Data.MySqlClient;
using ProjetoIntegradorVendas.Classes; // Use o namespace correto para seus modelos
using System;

namespace ProjetoIntegradorVendas.Services
{
    public class ClienteService
    {
        public Cliente Autenticar(string nome, string senha)
        {
            // A query agora busca todas as colunas do cliente
            const string query = "SELECT * FROM cliente WHERE ClienteNome = @nome AND Senha = @senha";

            using var conn = Database.GetConnection();
            try
            {
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@senha", senha);

                using var reader = cmd.ExecuteReader();

                // Se encontrou um registro, constrói e retorna o objeto Cliente
                if (reader.Read())
                {
                    return ConstruirCliente(reader);
                }

                // Se não encontrou, retorna nulo
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao autenticar cliente: {ex.Message}");
                throw;
            }
        }

        private Cliente ConstruirCliente(MySqlDataReader reader)
        {
            return new Cliente
            {
                ClienteID = reader.GetInt32("ClienteID"),
                CidadeID = reader.IsDBNull(reader.GetOrdinal("CidadeID")) ? 0 : reader.GetInt32("CidadeID"),
                ClienteNome = reader.GetString("ClienteNome"),
                ClienteCPF = reader.GetString("ClienteCPF"),
                Senha = reader.GetString("Senha"),
                ClienteCep = reader.GetString("ClienteCep"),
                ClienteDataNascimento = reader.GetDateTime("ClienteDataNascimento"),
                ClienteTelefone = reader.GetString("ClienteTelefone"),
                ClienteSexo = reader.GetChar("ClienteSexo")
            };
        }
    }
}