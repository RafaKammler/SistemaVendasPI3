using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
using ProjetoIntegradorVendas.Classes;
using ProjetoIntegradorVendas.Services;
using Wpf.Ui.Controls;
using Button = System.Windows.Controls.Button;

namespace ProjetoIntegradorVendas
{
    /// <summary>
    /// Interaction logic for CatalogoProdutosPage.xaml
    /// </summary>
    public partial class CatalogoProdutosPage : Page
    {

        private Classes.Cliente usuario;
        public ObservableCollection<Produto> Produtos { get; set; }
        private readonly CarrinhoService _carrinhoService = new CarrinhoService();
        public CatalogoProdutosPage(Classes.Cliente usuario)
        {
            InitializeComponent();

            var service = new ProdutoService();
            Produtos = service.BuscarProdutos();
            this.DataContext = this;
            this.usuario = usuario;

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
                        mainWindow.MainFrame.Navigate(new CatalogoProdutosPage(usuario));
                        break;
                    case "Carrinho":
                        AbrirFlyoutCarrinho(mainWindow);
                        break;
                    case "Configurações":
                        mainWindow.MainFrame.Navigate(new CatalogoProdutosPage(usuario));
                        break;
                    case "Logout":
                        mainWindow.MainFrame.Navigate(new LoginPage());
                        break;
                }
            }
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (sender is Border border && border.Tag is Produto produto)
            {
                mainWindow.MainFrame.Navigate(new DetalheProdutoPage(produto, usuario));
            }
        }

        private void AbrirFlyoutCarrinho(MainWindow mainWindow)
        {
            var carrinhoService = new CarrinhoService();
            var itensCarrinho = carrinhoService.ObterItensDoCarrinho(this.usuario.ClienteID);

            double valorTotal = 0;
            foreach (var item in itensCarrinho)
            {
                valorTotal += item.Produto.Preco * item.Quantidade;
            }

            var carrinhoControl = new ProjetoIntegradorVendas.Cliente.CarrinhoControl();
            carrinhoControl.CartItemsListView.ItemsSource = itensCarrinho;
            carrinhoControl.TotalCarrinho.Text = $"Total: {valorTotal:C}"; // Formata como moeda

            mainWindow.CartFlyout.Content = carrinhoControl;
            mainWindow.CartFlyout.IsOpen = true;
        }
        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Produto produto)
            {
                try
                {
                    // Utiliza o serviço para adicionar o item ao carrinho
                    _carrinhoService.AdicionarAoCarrinho(usuario.ClienteID, produto.Id, 1);

                    MostrarSnackbar($"'{produto.Nome}' foi adicionado ao carrinho!", ControlAppearance.Success);
                }
                catch (Exception ex)
                {
                    MostrarSnackbar("Não foi possível adicionar o produto ao carrinho.", ControlAppearance.Danger);
                    // É uma boa prática logar o erro para depuração
                    Console.WriteLine($"Erro ao adicionar item ao carrinho: {ex.Message}");
                }
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
    }
}


