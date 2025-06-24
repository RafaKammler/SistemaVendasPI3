using ProjetoIntegradorVendas.Classes;
using ProjetoIntegradorVendas.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;
using System;

namespace ProjetoIntegradorVendas.Vendedor
{
    public partial class DetalheProdutoVendedorPage : Page
    {
        public Produto Produto { get; set; }
        private Fornecedor VendedorLogado { get; set; }
        public ObservableCollection<Comentario> Comentarios { get; set; }
        private readonly ComentarioService _comentarioService = new ComentarioService();

        public DetalheProdutoVendedorPage(Produto produto, Fornecedor vendedor)
        {
            InitializeComponent();
            this.Produto = produto;
            this.VendedorLogado = vendedor;
            this.DataContext = this;

            CarregarComentarios();
        }

        private void CarregarComentarios()
        {
            try
            {
                Comentarios = _comentarioService.BuscarComentariosPorProduto(this.Produto.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao carregar comentários: {ex.Message}");
                Comentarios = new ObservableCollection<Comentario>();
            }
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
                        mainWindow.MainFrame.Navigate(new CatalogoVendedorPage(VendedorLogado));
                        break;
                    case "Logout":
                        mainWindow.MainFrame.Navigate(new LoginPage());
                        break;
                }
            }
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.MainFrame.Navigate(new EditarProduoPage(this.VendedorLogado, this.Produto));
        }
    }
}