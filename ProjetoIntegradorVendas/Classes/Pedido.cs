namespace ProjetoIntegradorVendas.Classes;

public class Pedido
{
    public int PedidoID { get; set; }
    public int ClienteID { get; set; }
    public DateTime DataPedido { get; set; }
    public decimal VendaValor { get; set; }
}