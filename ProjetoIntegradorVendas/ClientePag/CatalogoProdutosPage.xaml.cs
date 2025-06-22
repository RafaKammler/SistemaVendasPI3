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

namespace ProjetoIntegradorVendas
{
    /// <summary>
    /// Interaction logic for CatalogoProdutosPage.xaml
    /// </summary>
    public partial class CatalogoProdutosPage : Page
    {

        private Classes.Cliente usuario;
        public ObservableCollection<Produto> Produtos { get; set; }
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
    }
}
