using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjetoIntegradorVendas.Services;
using ProjetoIntegradorVendas.Vendedor;
using Wpf.Ui.Controls;
using Wpf.Ui.Interop.WinDef;

namespace ProjetoIntegradorVendas
{
    /// <summary>
    /// Interaction logic for CadastroProdutosPage.xaml
    /// </summary>
   
    public partial class EditarProduoPage : Page
    {
        public Produto Produto { get; set; }

        private Fornecedor vendedorId;

        public EditarProduoPage(Fornecedor vendedorId, Produto produto)
        {
            InitializeComponent();

            this.vendedorId = vendedorId;
            this.Produto = produto;
            this.DataContext = this;
        }

        private void NavigationView_OnItemInvoked(object sender, RoutedEventArgs e)
        {
            if (sender is NavigationViewItem selectedItem)
            {
                string content = selectedItem.Content.ToString();
                var mainWindow = (MainWindow)Application.Current.MainWindow;

                switch (content)
                {
                    case "Home":
                        mainWindow.MainFrame.Navigate(new CatalogoVendedorPage(vendedorId));
                        break;
                    case "Logout":
                        mainWindow.MainFrame.Navigate(new LoginPage());
                        break;
                }
            }

        }
        private void CadastrarProduto_Click(object sender, RoutedEventArgs e)
        {
            string nomeProduto = txNomeProduto.Text;
            string descricaoProduto = txDescricaoProduto.Text;

            if (!Double.TryParse(txPrecoProduto.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double preco))
            {
                MostrarSnackbar("O preço do produto deve ser um número válido.", ControlAppearance.Danger);
                return;
            }

            if (!int.TryParse(txEstoqueProduto.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out int estoque))
            {
                MostrarSnackbar("O estoque deve ser um número inteiro.", ControlAppearance.Danger);
                return;
            }

            var fornecedorService = new FornecedorService();
            Fornecedor fornecedor = fornecedorService.encontrarFornecedor(vendedorId);

            if (fornecedor == null)
            {
                MostrarSnackbar("Fornecedor não encontrado.", ControlAppearance.Danger);
                return;
            }

            if (string.IsNullOrEmpty(nomeProduto))
            {
                MostrarSnackbar("O nome do produto é obrigatório.", ControlAppearance.Danger);
                return;
            }

            if (string.IsNullOrEmpty(descricaoProduto))
            {
                MostrarSnackbar("A descrição do produto é obrigatória.", ControlAppearance.Danger);
                return;
            }

            var produto = new Produto
            {
                IdFornecedor = fornecedor,
                Nome = nomeProduto,
                Descricao = descricaoProduto,
                Preco = preco,
                Imagem = "",
                Estoque = estoque,
                ImagemPath = null 
            };

            var service = new ProdutoService();
            try
            {
                service.CadastrarProduto(produto);
                MostrarSnackbar("Produto cadastrado com sucesso.", ControlAppearance.Success);
                txNomeProduto.Clear();
                txDescricaoProduto.Clear();
                txPrecoProduto.Clear();
                txEstoqueProduto.Clear();
            }
            catch (Exception ex)
            {
                MostrarSnackbar("Erro ao cadastrar produto: " + ex.Message, ControlAppearance.Danger);
            }
        }

        public void MostrarSnackbar(string mensagem, ControlAppearance aparencia)
        {
            Snackbar dlgMsg = new Snackbar(RootSnackbarPresenter);
            dlgMsg.Appearance = aparencia;
            dlgMsg.Title = new System.Windows.Controls.TextBlock
            {
                Text = mensagem,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                FontWeight = FontWeights.SemiBold
            };
            dlgMsg.IsCloseButtonEnabled = false;


            dlgMsg.Show();
        }
        private void NumberBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }
}
