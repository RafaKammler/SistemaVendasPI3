using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Media.Imaging;

namespace ProjetoIntegradorVendas.Services
{
    public class ProdutoService
    {
        public ObservableCollection<Produto> BuscarProdutos()
        {
            var produtos = new ObservableCollection<Produto>();

            using (var conn = Database.GetConnection())
            {
                try
                {
                    conn.Open();
                    var query = @"
    SELECT 
        p.ProdutoID, 
        p.FornecedorID, 
        p.Descricao, 
        p.Preco, 
        p.Imagem, 
        p.Nome,
        f.FornecedorID AS FID, 
        f.CidadeID, 
        f.FornecedorNome, 
        f.FornecedorCNPJ, 
        f.Senha, 
        f.FornecedorCEP, 
        f.FornecedorEmail, 
        f.FornecedorTelefone
    FROM produto p
    JOIN fornecedor f ON p.FornecedorID = f.FornecedorID";


                    var cmd = new MySqlCommand(query, conn);

                    using var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        var fornecedor = new Fornecedor
                        {
                            FornecedorID = reader.GetInt32("FID"),
                            FornecedorNome = reader.GetString("FornecedorNome"),
                            FornecedorEmail = reader.GetString("FornecedorEmail"),
                            FornecedorCNPJ = reader.GetString("FornecedorCNPJ"),
                            FornecedorCEP = reader.GetString("FornecedorCEP"),
                            FornecedorTelefone = reader.GetString("FornecedorTelefone")
                        };

                        var produto = new Produto
                        {
                            Id = reader.GetInt32("ProdutoID"),
                            IdFornecedor = fornecedor,
                            Nome = reader.GetString("Nome"),
                            Descricao = reader.GetString("Descricao"),
                            Preco =  reader.GetDouble("Preco"),
                            Imagem = reader.GetString("Imagem")
                        };
                        string caminhoRelativo = produto.Imagem.TrimStart('/');
                        string pastaBase = AppDomain.CurrentDomain.BaseDirectory;
                        string caminhoCompleto = System.IO.Path.Combine(pastaBase, caminhoRelativo);

                        var uri = new Uri(caminhoCompleto, UriKind.Absolute);
                        produto.ImagemPath = new BitmapImage(uri);

                        produtos.Add(produto);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao buscar produtos: {ex.Message}");
                    throw ex;
                }
            }

            return produtos;
        }
    }
}
