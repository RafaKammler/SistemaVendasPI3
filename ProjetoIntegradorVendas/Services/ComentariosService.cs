using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ProjetoIntegradorVendas.Classes;

namespace ProjetoIntegradorVendas.Services
{
    public class ComentarioService
    {
        /// <summary>
        /// Busca todos os comentários associados a um ID de produto específico.
        /// </summary>
        /// <param name="produtoId">O ID do produto cujos comentários serão buscados.</param>
        /// <returns>Uma coleção de objetos Comentario.</returns>
        public ObservableCollection<Comentario> BuscarComentariosPorProduto(int produtoId)
        {
            var comentarios = new ObservableCollection<Comentario>();

            // A query junta a tabela Comentario com a Cliente para obter o nome de quem comentou.
            const string query = @"
                SELECT 
                    co.ComentarioID,
                    co.ComentarioTexto,
                    co.DataComentario,
                    cl.ClienteID,
                    cl.ClienteNome
                FROM 
                    Comentario co
                JOIN 
                    Cliente cl ON co.ClienteID = cl.ClienteID
                WHERE 
                    co.ProdutoID = @ProdutoID
                ORDER BY 
                    co.DataComentario DESC;";

            using var conn = Database.GetConnection();
            try
            {
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProdutoID", produtoId);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    // Usa o método auxiliar para construir o objeto
                    var comentario = ConstruirComentario(reader);
                    comentarios.Add(comentario);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar comentários: {ex.Message}");
                // É uma boa prática relançar a exceção ou logar de forma mais robusta.
                throw;
            }

            return comentarios;
        }

        /// <summary>
        /// Salva um novo comentário no banco de dados.
        /// </summary>
        /// <param name="comentario">O objeto Comentario a ser salvo.</param>
        public void SalvarComentario(Comentario comentario)
        {
            // Query para inserir um novo registro na tabela.
            const string query = @"
                INSERT INTO Comentario (ProdutoID, ClienteID, ComentarioTexto, DataComentario)
                VALUES (@ProdutoID, @ClienteID, @ComentarioTexto, @DataComentario);";

            using var conn = Database.GetConnection();
            try
            {
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);

                // Adiciona os parâmetros para evitar SQL Injection.
                cmd.Parameters.AddWithValue("@ProdutoID", comentario.ProdutoID);
                cmd.Parameters.AddWithValue("@ClienteID", comentario.ClienteID);
                cmd.Parameters.AddWithValue("@ComentarioTexto", comentario.ComentarioTexto);
                cmd.Parameters.AddWithValue("@DataComentario", DateTime.Now); // Usa a data e hora atuais.

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar comentário: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Método auxiliar para criar um objeto Comentario a partir de um MySqlDataReader.
        /// </summary>
        private Comentario ConstruirComentario(MySqlDataReader reader)
        {
            // Cria um objeto Cliente apenas com as informações necessárias (ID e Nome).
            var cliente = new Classes.Cliente
            {
                ClienteID = reader.GetInt32("ClienteID"),
                ClienteNome = reader.GetString("ClienteNome")
            };

            // Cria o objeto Comentario completo.
            return new Comentario
            {
                ComentarioID = reader.GetInt32("ComentarioID"),
                ComentarioTexto = reader.GetString("ComentarioTexto"),
                DataComentario = reader.GetDateTime("DataComentario"),
                Cliente = cliente, // Associa o objeto Cliente ao comentário.
                ClienteID = cliente.ClienteID
            };
        }
    }
}
