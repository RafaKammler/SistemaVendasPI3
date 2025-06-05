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
using ProjetoIntegradorVendas.Vendedor;
using Wpf.Ui.Controls;

namespace ProjetoIntegradorVendas
{
    /// <summary>
    /// Interaction logic for DetalheProdutoVendedorPage.xaml
    /// </summary>
    public partial class DetalheProdutoVendedorPage : Page
    {
        public Produto Produto { get; set; }
        private int VendedorID {get; set; }
        public DetalheProdutoVendedorPage(Produto produto, int vendedorID)
        {
            InitializeComponent();
            this.Produto = produto;
            this.DataContext = this;
            this.VendedorID = vendedorID;
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
                        mainWindow.MainFrame.Navigate(new CatalogoVendedorPage(VendedorID));
                        break;
                    case "Logout":
                        mainWindow.MainFrame.Navigate(new LoginPage());
                        break;
                }
            }
        }
    }
}
