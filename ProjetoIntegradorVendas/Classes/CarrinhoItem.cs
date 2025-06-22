namespace ProjetoIntegradorVendas.Classes
{
    public class CarrinhoItem
    {
        public int CarrinhoItemID { get; set; }
        public int ClienteID { get; set; }
        public int ProdutoID { get; set; }
        public int Quantidade { get; set; }
        public DateTime DataAdicionado { get; set; }

        public Produto Produto { get; set; }
    }
}