using MySql.Data.MySqlClient;
using System;

namespace ProjetoIntegradorVendas.Services
{
    public class FornecedorService
    {
        public Fornecedor encontrarFornecedor(Fornecedor fornecedorId)
        {
            const string query = @"
                SELECT 
                    f.FornecedorID,
                    f.FornecedorNome,
                    f.FornecedorEmail,
                    f.FornecedorCNPJ,
                    f.FornecedorCEP,
                    f.FornecedorTelefone
                FROM fornecedor f
                WHERE f.FornecedorID = @FornecedorID;";

            using var conn = Database.GetConnection();
            try
            {
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FornecedorID", fornecedorId.FornecedorID);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return ConstruirFornecedor(reader);
                }
                else
                {
                    return null!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar fornecedor: {ex.Message}");
                throw;
            }
        }
        public Fornecedor Autenticar(string nome, string senha)
        {
            const string query = "SELECT * FROM fornecedor WHERE FornecedorNome = @nome AND Senha = @senha";

            using var conn = Database.GetConnection();
            try
            {
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@senha", senha);

                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return ConstruirFornecedor(reader);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao autenticar fornecedor: {ex.Message}");
                throw;
            }
        }
        private Fornecedor ConstruirFornecedor(MySqlDataReader reader)
        {
            return new Fornecedor
            {
                FornecedorID = reader.GetInt32("FornecedorID"),
                FornecedorNome = reader.GetString("FornecedorNome"),
                FornecedorEmail = reader.GetString("FornecedorEmail"),
                FornecedorCNPJ = reader.GetString("FornecedorCNPJ"),
                FornecedorCEP = reader.GetString("FornecedorCEP"),
                FornecedorTelefone = reader.GetString("FornecedorTelefone")
            };
        }
    }
}
