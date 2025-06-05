using System.Windows.Media.Imaging;

namespace ProjetoIntegradorVendas;

public class Produto
{
    public int Id { get; set; }
    public Fornecedor IdFornecedor { get; set; }
    public string Nome { get; set; }
    public double Preco { get; set; }
    public string Descricao { get; set; }
    public string Imagem { get; set; }
    public BitmapImage ImagemPath { get; set; }
    public int Estoque { get; set; }
}