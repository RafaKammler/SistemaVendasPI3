using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
using Wpf.Ui.Controls;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace ProjetoIntegradorVendas.Vendedor
{
    /// <summary>
    /// Interaction logic for CatalogoVendedorPage.xaml
    /// </summary>
    public partial class CatalogoVendedorPage : Page
    {
        public ObservableCollection<Produto> Produtos { get; set; }
        private Fornecedor vendedorId { get; set; }

        public CatalogoVendedorPage(Fornecedor idVendedor)
        {
            InitializeComponent();

            var service = new ProdutoService();
            Produtos = service.BuscarProdutosVendedor(idVendedor);
            this.DataContext = this;
            this.vendedorId = idVendedor;
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

        public void AbrirDetalhes_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button selectedItem)
            {
                string content = selectedItem.Content.ToString();
                var mainWindow = (MainWindow)Application.Current.MainWindow;

                mainWindow.MainFrame.Navigate(new CadastroProdutosPage(vendedorId));


            }
        }
        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (sender is Border border && border.Tag is Produto produto)
            {
                mainWindow.MainFrame.Navigate(new DetalheProdutoVendedorPage(produto, vendedorId));
            }
        }

        public void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button selectedItem)
            {
                string content = selectedItem.Content.ToString();
                var mainWindow = (MainWindow)Application.Current.MainWindow;

                mainWindow.MainFrame.Navigate(new CadastroProdutosPage(vendedorId));


            }
        }

        public void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Produto produtoSelecionado)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.MainFrame.Navigate(new EditarProduoPage(vendedorId, produtoSelecionado));
            }
        }
        public void Deletar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Produto produtoParaDeletar)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Você tem certeza que deseja deletar o produto '{produtoParaDeletar.Nome}'?\nEsta ação não pode ser desfeita.",
                    "Confirmar Exclusão",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var service = new ProdutoService();
                    try
                    {
                        service.DeletarProduto(produtoParaDeletar.Id);

                        Produtos.Remove(produtoParaDeletar);

                        MessageBox.Show("Produto deletado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao deletar o produto: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
