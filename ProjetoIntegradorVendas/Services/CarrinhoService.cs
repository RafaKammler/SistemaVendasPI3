using MySql.Data.MySqlClient;
using ProjetoIntegradorVendas.Classes;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace ProjetoIntegradorVendas.Services
{
    public class CarrinhoService
    {
        public void AdicionarAoCarrinho(int clienteId, int produtoId, int quantidade)
        {
            const string query = @"
                INSERT INTO carrinho_item (ClienteID, ProdutoID, Quantidade, DataAdicionado)
                VALUES (@ClienteID, @ProdutoID, @Quantidade, NOW())
                ON DUPLICATE KEY UPDATE Quantidade = Quantidade + @Quantidade;";

            using var conn = Database.GetConnection();
            try
            {
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ClienteID", clienteId);
                cmd.Parameters.AddWithValue("@ProdutoID", produtoId);
                cmd.Parameters.AddWithValue("@Quantidade", quantidade);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar item ao carrinho: {ex.Message}");
                throw;
            }
        }

        public void RemoverDoCarrinho(int carrinhoItemId)
        {
            const string query = "DELETE FROM carrinho_item WHERE CarrinhoItemID = @CarrinhoItemID;";

            using var conn = Database.GetConnection();
            try
            {
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@CarrinhoItemID", carrinhoItemId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao remover item do carrinho: {ex.Message}");
                throw;
            }
        }

        public ObservableCollection<CarrinhoItem> ObterItensDoCarrinho(int clienteId)
        {
            var itens = new ObservableCollection<CarrinhoItem>();
            const string query = @"
                SELECT 
                    ci.CarrinhoItemID, ci.Quantidade, ci.DataAdicionado,
                    p.ProdutoID, p.Descricao, p.Preco, p.Imagem, p.Nome, p.Estoque,
                    f.FornecedorID AS FID, f.FornecedorNome, f.FornecedorCNPJ, f.Senha, f.FornecedorCEP, f.FornecedorEmail, f.FornecedorTelefone
                FROM 
                    carrinho_item ci
                JOIN 
                    produto p ON ci.ProdutoID = p.ProdutoID
                JOIN 
                    fornecedor f ON p.FornecedorID = f.FornecedorID
                WHERE 
                    ci.ClienteID = @ClienteID
                ORDER BY
                    ci.DataAdicionado DESC;";

            using var conn = Database.GetConnection();
            try
            {
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClienteID", clienteId);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var fornecedor = ConstruirFornecedor(reader);
                    var produto = ConstruirProduto(reader, fornecedor);

                    var carrinhoItem = new CarrinhoItem
                    {
                        CarrinhoItemID = reader.GetInt32("CarrinhoItemID"),
                        ClienteID = clienteId,
                        ProdutoID = produto.Id,
                        Quantidade = reader.GetInt32("Quantidade"),
                        DataAdicionado = reader.GetDateTime("DataAdicionado"),
                        Produto = produto
                    };
                    itens.Add(carrinhoItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter itens do carrinho: {ex.Message}");
                throw;
            }

            return itens;
        }

        public void LimparCarrinho(int clienteId)
        {
            const string query = "DELETE FROM carrinho_item WHERE ClienteID = @ClienteID;";

            using var conn = Database.GetConnection();
            try
            {
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ClienteID", clienteId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar o carrinho: {ex.Message}");
                throw;
            }
        }

        private Fornecedor ConstruirFornecedor(MySqlDataReader reader)
        {
            return new Fornecedor
            {
                FornecedorID = reader.GetInt32("FID"),
                FornecedorNome = reader.GetString("FornecedorNome"),
                FornecedorEmail = reader.GetString("FornecedorEmail"),
                FornecedorCNPJ = reader.GetString("FornecedorCNPJ"),
                FornecedorCEP = reader.GetString("FornecedorCEP"),
                FornecedorTelefone = reader.GetString("FornecedorTelefone")
            };
        }

        private Produto ConstruirProduto(MySqlDataReader reader, Fornecedor fornecedor)
        {
            string imagem = reader.IsDBNull(reader.GetOrdinal("Imagem")) ? "" : reader.GetString("Imagem");
            string nome = reader.IsDBNull(reader.GetOrdinal("Nome")) ? "" : reader.GetString("Nome");
            string descricao = reader.IsDBNull(reader.GetOrdinal("Descricao")) ? "" : reader.GetString("Descricao");
            double preco = reader.IsDBNull(reader.GetOrdinal("Preco")) ? 0.0 : reader.GetDouble("Preco");
            int estoque = reader.IsDBNull(reader.GetOrdinal("Estoque")) ? 0 : reader.GetInt32("Estoque");

            string imagemRelativa = imagem.TrimStart('/');
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string caminhoCompleto = Path.Combine(basePath, imagemRelativa);

            var imagemPath = File.Exists(caminhoCompleto)
                ? new BitmapImage(new Uri(caminhoCompleto, UriKind.Absolute))
                : null;

            return new Produto
            {
                Id = reader.GetInt32("ProdutoID"),
                IdFornecedor = fornecedor,
                Nome = nome,
                Descricao = descricao,
                Preco = preco,
                Imagem = imagem,
                Estoque = estoque,
                ImagemPath = imagemPath
            };
        }
    }
}