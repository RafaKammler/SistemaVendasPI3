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

        private Cliente usuario;
        public ObservableCollection<Produto> Produtos { get; set; }
        public CatalogoProdutosPage(Cliente usuario)
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
                        mainWindow.MainFrame.Navigate(new CatalogoProdutosPage(usuario));
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

        private void NavigationViewItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NavigationViewItem_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (sender is Border border && border.Tag is Produto produto)
            {
                mainWindow.MainFrame.Navigate(new DetalheProdutoPage(produto, usuario));
            }
        }
    }
}
