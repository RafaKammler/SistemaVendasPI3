using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySqlX.XDevAPI;
using ProjetoIntegradorVendas.Classes;
using ProjetoIntegradorVendas.ClientePag;
using ProjetoIntegradorVendas.Services;
using Wpf.Ui.Controls;
using Wpf.Ui.Input;

namespace ProjetoIntegradorVendas
{
    public partial class DetalheProdutoPage : Page, INotifyPropertyChanged
    {
        private readonly Classes.Cliente _clienteLogado;
        private readonly ComentarioService _comentarioService = new ComentarioService();
        private readonly CarrinhoService _carrinhoService = new CarrinhoService();

        public Produto Produto { get; set; }

        public ObservableCollection<Comentario> Comentarios { get; set; }

        private string _novoComentarioTexto;
        public string NovoComentarioTexto
        {
            get => _novoComentarioTexto;
            set
            {
                _novoComentarioTexto = value;
                OnPropertyChanged(nameof(NovoComentarioTexto));
            }
        }

        public ICommand SalvarComentarioCommand { get; }
        public ICommand ComprarCommand { get; }
        public ICommand AdicionarCarrinhoCommand { get; }

        public DetalheProdutoPage(Produto produto, Classes.Cliente cliente)
        {
            InitializeComponent();

            this.Produto = produto;
            this._clienteLogado = cliente;

            // Inicialize os comandos no construtor
            SalvarComentarioCommand = new RelayCommand<object>(ExecutarSalvarComentario);
            ComprarCommand = new RelayCommand<object>(ExecutarComprarAgora);
            AdicionarCarrinhoCommand = new RelayCommand<object>(ExecutarAdicionarCarrinho);

            this.DataContext = this;
            CarregarComentarios();
        }

        private void CarregarComentarios()
        {
            try
            {
                var comentariosDoBanco = _comentarioService.BuscarComentariosPorProduto(this.Produto.Id);
                Comentarios = new ObservableCollection<Comentario>(comentariosDoBanco);
                OnPropertyChanged(nameof(Comentarios)); // Notifica a UI que a coleção foi carregada
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao carregar comentários: {ex.Message}");
                Comentarios = new ObservableCollection<Comentario>();
            }
        }

        private void ExecutarSalvarComentario(object parameter)
        {
            if (string.IsNullOrWhiteSpace(NovoComentarioTexto)) return;

            var novoComentario = new Comentario
            {
                ProdutoID = this.Produto.Id,
                ClienteID = this._clienteLogado.ClienteID,
                ComentarioTexto = this.NovoComentarioTexto,
                DataComentario = DateTime.Now,

                Cliente = this._clienteLogado
            };

            try
            {
                _comentarioService.SalvarComentario(novoComentario);

                Comentarios.Insert(0, novoComentario);
                NovoComentarioTexto = string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao salvar comentário: {ex.Message}");
            }
        }

        private void ExecutarComprarAgora(object parameter)
        {
            try
            {
                _carrinhoService.AdicionarAoCarrinho(_clienteLogado.ClienteID, Produto.Id, 1);

                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.MainFrame.Navigate(new ConfirmarCompraPage(_clienteLogado));
            }
            catch (Exception ex)
            {
                MostrarSnackbar("Erro ao iniciar a compra.", ControlAppearance.Danger);
                Console.WriteLine($"Erro no ComprarAgora: {ex.Message}");
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
                        mainWindow.MainFrame.Navigate(new CatalogoProdutosPage(_clienteLogado));
                        break;
                    case "Carrinho":
                        AbrirFlyoutCarrinho(mainWindow);
                        break;
                    case "Configurações":
                        break;
                    case "Logout":
                        mainWindow.MainFrame.Navigate(new LoginPage());
                        break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ExecutarAdicionarCarrinho(object parameter)
        {
            if (this.Produto == null || _clienteLogado == null)
            {
                return;
            }

            try
            {
                var carrinhoService = new CarrinhoService();

                carrinhoService.AdicionarAoCarrinho(_clienteLogado.ClienteID, this.Produto.Id, 1);

                MostrarSnackbar("Produto adicionado ao carrinho.",
                    Wpf.Ui.Controls.ControlAppearance.Success);
            }
            catch (Exception ex)
            {
                MostrarSnackbar("Não foi possível adicionar o produto ao carrinho.",
                    Wpf.Ui.Controls.ControlAppearance.Danger);
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
        private void AbrirFlyoutCarrinho(MainWindow mainWindow)
        {
            var carrinhoService = new CarrinhoService();
            var itensCarrinho = carrinhoService.ObterItensDoCarrinho(this._clienteLogado.ClienteID);

            double valorTotal = 0;
            foreach (var item in itensCarrinho)
            {
                valorTotal += item.Produto.Preco * item.Quantidade;
            }

            var carrinhoControl = new ProjetoIntegradorVendas.Cliente.CarrinhoControl();
            carrinhoControl.CartItemsListView.ItemsSource = itensCarrinho;
            carrinhoControl.TotalCarrinho.Text = $"Total: {valorTotal:C}";

            mainWindow.CartFlyout.Content = carrinhoControl;
            mainWindow.CartFlyout.IsOpen = true;
        }
    }
}