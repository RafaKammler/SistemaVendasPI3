using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace ProjetoIntegradorVendas.Services
{
    public class ProdutoService
    {
        private const string sqlProdutosBase = @"
            SELECT 
                p.ProdutoID, 
                p.FornecedorID, 
                p.Descricao, 
                p.Preco, 
                p.Imagem, 
                p.Nome,
                p.Estoque,
                f.FornecedorID AS FID, 
                f.CidadeID, 
                f.FornecedorNome, 
                f.FornecedorCNPJ, 
                f.Senha, 
                f.FornecedorCEP, 
                f.FornecedorEmail, 
                f.FornecedorTelefone
            FROM 
                produto p
            JOIN 
                fornecedor f ON p.FornecedorID = f.FornecedorID";

        public ObservableCollection<Produto> BuscarProdutos()
        {
            return ExecutarConsultaProdutos(sqlProdutosBase);
        }

        public ObservableCollection<Produto> BuscarProdutosVendedor(Fornecedor codigoVendedor)
        {
            string query = sqlProdutosBase + " WHERE f.FornecedorID = @codigoVendedor";
            var parametros = new Dictionary<string, object> { { "@codigoVendedor", codigoVendedor.FornecedorID } };
            return ExecutarConsultaProdutos(query, parametros);
        }

        private ObservableCollection<Produto> ExecutarConsultaProdutos(string query, Dictionary<string, object>? parametros = null)
        {
            var produtos = new ObservableCollection<Produto>();

            using var conn = Database.GetConnection();
            try
            {
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);

                if (parametros != null)
                {
                    foreach (var param in parametros)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var fornecedor = ConstruirFornecedor(reader);
                    var produto = ConstruirProduto(reader, fornecedor);
                    produtos.Add(produto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar produtos: {ex.Message}");
                throw;
            }

            return produtos;
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
        public void CadastrarProduto(Produto produto)
        {
            const string query = @"
            INSERT INTO produto (FornecedorID, Nome, Descricao, Preco, Imagem, Estoque)
            VALUES (@FornecedorID, @Nome, @Descricao, @Preco, @Imagem, @Estoque);";

            using var conn = Database.GetConnection();
            try
            {
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@FornecedorID", produto.IdFornecedor?.FornecedorID ?? 0);
                cmd.Parameters.AddWithValue("@Nome", produto.Nome ?? string.Empty);
                cmd.Parameters.AddWithValue("@Descricao", produto.Descricao ?? string.Empty);
                cmd.Parameters.AddWithValue("@Preco", produto.Preco);
                cmd.Parameters.AddWithValue("@Imagem", produto.Imagem ?? string.Empty);
                cmd.Parameters.AddWithValue("@Estoque", produto.Estoque);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao cadastrar produto: {ex.Message}");
                throw;
            }
        }

        // NOVO MÉTODO ADICIONADO AQUI
        public void AtualizarProduto(Produto produto)
        {
            const string query = @"
                UPDATE produto SET
                    Nome = @Nome,
                    Descricao = @Descricao,
                    Preco = @Preco,
                    Estoque = @Estoque,
                    Imagem = @Imagem
                WHERE ProdutoID = @ProdutoID;";

            using var conn = Database.GetConnection();
            try
            {
                conn.Open();
                using var cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Nome", produto.Nome);
                cmd.Parameters.AddWithValue("@Descricao", produto.Descricao);
                cmd.Parameters.AddWithValue("@Preco", produto.Preco);
                cmd.Parameters.AddWithValue("@Estoque", produto.Estoque);
                cmd.Parameters.AddWithValue("@Imagem", produto.Imagem);
                cmd.Parameters.AddWithValue("@ProdutoID", produto.Id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar produto: {ex.Message}");
                throw;
            }
        }
        public void DeletarProduto(int produtoId)
        {
            using var conn = Database.GetConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();

            try
            {
                const string queryComentarios = "DELETE FROM Comentario WHERE ProdutoID = @ProdutoID";
                using (var cmdComentarios = new MySqlCommand(queryComentarios, conn, transaction))
                {
                    cmdComentarios.Parameters.AddWithValue("@ProdutoID", produtoId);
                    cmdComentarios.ExecuteNonQuery();
                }

                const string queryPedidoXProduto = "DELETE FROM pedidoxproduto WHERE ProdutoID = @ProdutoID";
                using (var cmdPedidoXProduto = new MySqlCommand(queryPedidoXProduto, conn, transaction))
                {
                    cmdPedidoXProduto.Parameters.AddWithValue("@ProdutoID", produtoId);
                    cmdPedidoXProduto.ExecuteNonQuery();
                }

                const string queryProduto = "DELETE FROM produto WHERE ProdutoID = @ProdutoID";
                using (var cmdProduto = new MySqlCommand(queryProduto, conn, transaction))
                {
                    cmdProduto.Parameters.AddWithValue("@ProdutoID", produtoId);
                    cmdProduto.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Erro ao deletar produto: {ex.Message}");
                throw;
            }
        }
    }
}