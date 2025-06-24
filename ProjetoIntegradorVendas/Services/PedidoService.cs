using MySql.Data.MySqlClient;
using ProjetoIntegradorVendas.Classes;

namespace ProjetoIntegradorVendas.Services;

public class PedidoService
{
    // Alterado para receber uma lista de CarrinhoItem
    public void CriarPedido(Pedido pedido, List<CarrinhoItem> itens)
    {
        using var conn = Database.GetConnection();
        conn.Open();
        using var transaction = conn.BeginTransaction();

        try
        {
            string queryPedido = "INSERT INTO pedido (ClienteID, VendaData, VendaValor, DataPedido, HoraPedido) VALUES (@ClienteID, @VendaData, @VendaValor, @DataPedido, @HoraPedido); SELECT LAST_INSERT_ID();";

            long pedidoId;
            using (var cmdPedido = new MySqlCommand(queryPedido, conn, transaction))
            {
                cmdPedido.Parameters.AddWithValue("@ClienteID", pedido.ClienteID);
                cmdPedido.Parameters.AddWithValue("@VendaData", pedido.DataPedido.Date);
                cmdPedido.Parameters.AddWithValue("@VendaValor", pedido.VendaValor);
                cmdPedido.Parameters.AddWithValue("@DataPedido", pedido.DataPedido.Date);
                cmdPedido.Parameters.AddWithValue("@HoraPedido", pedido.DataPedido.TimeOfDay);

                pedidoId = Convert.ToInt64(cmdPedido.ExecuteScalar());
            }

            string queryPedidoProduto = "INSERT INTO pedidoxproduto (PedidoID, ProdutoID, Quantidade) VALUES (@PedidoID, @ProdutoID, @Quantidade);";

            foreach (var item in itens)
            {
                using (var cmdPedidoProduto = new MySqlCommand(queryPedidoProduto, conn, transaction))
                {
                    cmdPedidoProduto.Parameters.AddWithValue("@PedidoID", pedidoId);
                    cmdPedidoProduto.Parameters.AddWithValue("@ProdutoID", item.ProdutoID); // Acessa o ID diretamente
                    cmdPedidoProduto.Parameters.AddWithValue("@Quantidade", item.Quantidade);
                    cmdPedidoProduto.ExecuteNonQuery();
                }
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine("Erro ao criar pedido: " + ex.Message);
            throw;
        }
    }
}