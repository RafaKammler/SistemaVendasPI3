namespace ProjetoIntegradorVendas
{
    public class Fornecedor
    {
        public int FornecedorID { get; set; }
        public Cidade CidadeID { get; set; }
        public string FornecedorNome { get; set; }
        public string FornecedorCNPJ { get; set; }
        public string Senha { get; set; }
        public string FornecedorCEP { get; set; }
        public string FornecedorEmail { get; set; }
        public string FornecedorTelefone { get; set; }
        public List<Produto> Produtos { get; set; }
    }
}