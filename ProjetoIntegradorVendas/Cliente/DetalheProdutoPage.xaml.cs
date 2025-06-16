using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySqlX.XDevAPI;
using ProjetoIntegradorVendas.Classes;
using ProjetoIntegradorVendas.Services;
using Wpf.Ui.Controls;
using Wpf.Ui.Input;

namespace ProjetoIntegradorVendas
{
    public partial class DetalheProdutoPage : Page, INotifyPropertyChanged
    {
        private readonly Cliente _clienteLogado;
        private readonly ComentarioService _comentarioService = new ComentarioService();

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

        public DetalheProdutoPage(Produto produto, Cliente cliente)
        {
            InitializeComponent();

            this.Produto = produto;
            this._clienteLogado = cliente;
            SalvarComentarioCommand = new RelayCommand<object>(ExecutarSalvarComentario);

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

                // Agora o objeto está completo e a UI será atualizada corretamente.
                Comentarios.Insert(0, novoComentario);
                NovoComentarioTexto = string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao salvar comentário: {ex.Message}");
            }
        }

        // --- Navegação e Notificação de Propriedades ---

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
                        // Implementar navegação para o carrinho
                        break;
                    case "Configurações":
                        // Implementar navegação para configurações
                        break;
                    case "Logout":
                        mainWindow.MainFrame.Navigate(new LoginPage());
                        break;
                }
            }
        }

        // 6. Implementação necessária para o INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}