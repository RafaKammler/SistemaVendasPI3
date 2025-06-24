using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using ProjetoIntegradorVendas.Classes;
using ProjetoIntegradorVendas.Services;
using Wpf.Ui.Controls;

namespace ProjetoIntegradorVendas.ClientePag
{
    /// <summary>
    /// Interaction logic for ConfirmarCompraPage.xaml
    /// </summary>
    public partial class ConfirmarCompraPage : Page, INotifyPropertyChanged
    {
        // Alterado para usar sua classe CarrinhoItem
        public ObservableCollection<CarrinhoItem> ItensDoCarrinho { get; set; }
        public Classes.Cliente ClienteLogado { get; set; }

        private double _valorTotal;
        public double ValorTotal
        {
            get => _valorTotal;
            set
            {
                _valorTotal = value;
                OnPropertyChanged(nameof(ValorTotal));
            }
        }

        // Instanciando seus serviços
        private readonly PedidoService _pedidoService = new PedidoService();
        private readonly CarrinhoService _carrinhoService = new CarrinhoService();

        public ConfirmarCompraPage(Classes.Cliente cliente)
        {
            InitializeComponent();
            this.DataContext = this;

            ClienteLogado = cliente;
            CarregarDadosCarrinho();
        }

        private void CarregarDadosCarrinho()
        {
            // Busca os itens do banco de dados usando seu serviço
            ItensDoCarrinho = _carrinhoService.ObterItensDoCarrinho(ClienteLogado.ClienteID);
            // Calcula o valor total
            ValorTotal = ItensDoCarrinho.Sum(item => item.Subtotal);

            OnPropertyChanged(nameof(ItensDoCarrinho));
        }

        private void FinalizarCompra_Click(object sender, RoutedEventArgs e)
        {
            if (!ItensDoCarrinho.Any())
            {
                MostrarSnackbar("Seu carrinho está vazio.", ControlAppearance.Danger);
                return;
            }

            var novoPedido = new Pedido
            {
                ClienteID = ClienteLogado.ClienteID,
                DataPedido = DateTime.Now,
                VendaValor = (decimal)ValorTotal
            };

            try
            {
                // Passa a lista de CarrinhoItem para o serviço de pedido
                _pedidoService.CriarPedido(novoPedido, ItensDoCarrinho.ToList());
                MostrarSnackbar("Compra realizada com sucesso!", ControlAppearance.Success);

                // Limpa o carrinho do cliente no banco de dados
                _carrinhoService.LimparCarrinho(ClienteLogado.ClienteID);
                ItensDoCarrinho.Clear();
                ValorTotal = 0;
                OnPropertyChanged(nameof(ItensDoCarrinho));

                (sender as System.Windows.Controls.Button).IsEnabled = false;
            }
            catch (Exception ex)
            {
                MostrarSnackbar("Erro ao finalizar a compra. Tente novamente.", ControlAppearance.Danger);
                Console.WriteLine(ex.Message);
            }
        }

        private void MostrarSnackbar(string mensagem, ControlAppearance aparencia)
        {
            var snackbar = new Snackbar(RootSnackbarPresenter)
            {
                Title = mensagem,
                Appearance = aparencia,
                IsCloseButtonEnabled = false,
            };
            snackbar.Show();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                        mainWindow.MainFrame.Navigate(new CatalogoProdutosPage(ClienteLogado));
                        break;
                    case "Carrinho":
                        break;
                    case "Configurações":
                        break;
                    case "Logout":
                        mainWindow.MainFrame.Navigate(new LoginPage());
                        break;
                }
            }
        }
    }
}
