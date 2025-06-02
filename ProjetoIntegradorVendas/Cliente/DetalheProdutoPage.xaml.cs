using System;
using System.Collections.Generic;
using System.Linq;
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
using Wpf.Ui.Controls;

namespace ProjetoIntegradorVendas
{
    /// <summary>
    /// Interaction logic for DetalheProdutoPage.xaml
    /// </summary>
    public partial class DetalheProdutoPage : Page
    {
        public Produto Produto { get; set; }
        public DetalheProdutoPage(Produto produto)
        {
            InitializeComponent();
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
                        mainWindow.MainFrame.Navigate(new CatalogoProdutosPage());
                        break;
                    case "Carrinho":
                        mainWindow.MainFrame.Navigate(new CatalogoProdutosPage());
                        break;
                    case "Configurações":
                        mainWindow.MainFrame.Navigate(new CatalogoProdutosPage());
                        break;
                    case "Logout":
                        mainWindow.MainFrame.Navigate(new LoginPage());
                        break;
                }
            }
        }
    }
}
