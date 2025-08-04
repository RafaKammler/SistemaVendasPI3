using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProjetoIntegradorVendas.Classes;
using ProjetoIntegradorVendas.Services;
using Wpf.Ui.Controls;
using Wpf.Ui.Input; // Adicione este using para o RelayCommand

namespace ProjetoIntegradorVendas.ClientePag
{
    public partial class ConfirmarCompraPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<CarrinhoItem> ItensDoCarrinho { get; set; }
        public Classes.Cliente ClienteLogado { get; set; }

        private double _subtotal;
        public double Subtotal
        {
            get => _subtotal;
            set { _subtotal = value; OnPropertyChanged(nameof(Subtotal)); CalcularValorTotal(); }
        }

        private decimal _valorFrete;
        public decimal ValorFrete
        {
            get => _valorFrete;
            set { _valorFrete = value; OnPropertyChanged(nameof(ValorFrete)); CalcularValorTotal(); }
        }

        private decimal _valorTotalFinal;
        public decimal ValorTotalFinal
        {
            get => _valorTotalFinal;
            set { _valorTotalFinal = value; OnPropertyChanged(nameof(ValorTotalFinal)); }
        }

        private readonly PedidoService _pedidoService = new PedidoService();
        private readonly CarrinhoService _carrinhoService = new CarrinhoService();
        public ICommand BuscarCepCommand { get; }

        public ConfirmarCompraPage(Classes.Cliente cliente)
        {
            InitializeComponent();
            this.DataContext = this;

            ClienteLogado = cliente;
            BuscarCepCommand = new RelayCommand<object>(ExecutarBuscarViaCep);
            CarregarDadosCarrinho();
        }

        private void CarregarDadosCarrinho()
        {
            ItensDoCarrinho = _carrinhoService.ObterItensDoCarrinho(ClienteLogado.ClienteID);
            Subtotal = ItensDoCarrinho.Sum(item => item.Subtotal);
            OnPropertyChanged(nameof(ItensDoCarrinho));
        }

        private void CalcularValorTotal()
        {
            ValorTotalFinal = (decimal)Subtotal + ValorFrete;
        }

        private async void ExecutarBuscarViaCep(object parameter)
        {
            string cep = txCep.Text;
            if (!string.IsNullOrWhiteSpace(cep) && cep.Length == 8)
            {
                var cepService = new CepService();
                Endereco endereco = await cepService.BuscarEnderecoPorCep(cep);

                if (endereco != null)
                {
                    tbBairro.Text = endereco.Bairro;
                    tbCidade.Text = endereco.Localidade;
                    tbEstado.Text = endereco.Uf;
                    tbRua.Text = endereco.Logradouro;

                    var freteService = new FreteService();
                    ValorFrete = freteService.CalcularFrete(endereco);
                    tbValorFrete.Text = ValorFrete.ToString("C");

                    PainelResultados.Visibility = Visibility.Visible;
                }
                else
                {
                    MostrarSnackbar("CEP não encontrado ou inválido.", ControlAppearance.Danger);
                    PainelResultados.Visibility = Visibility.Collapsed;
                    ValorFrete = 0;
                    tbValorFrete.Text = "-";
                }
            }
            else
            {
                tbValorFrete.Text = "";
                PainelResultados.Visibility = Visibility.Hidden;
                MostrarSnackbar("Por favor, digite um CEP válido com 8 dígitos.", ControlAppearance.Info);
            }
        }

        private void FinalizarCompra_Click(object sender, RoutedEventArgs e)
        {
            if (!ItensDoCarrinho.Any())
            {
                MostrarSnackbar("Seu carrinho está vazio.", ControlAppearance.Danger);
                return;
            }
            if (ValorFrete <= 0 && tbValorFrete.Text == "-")
            {
                MostrarSnackbar("Por favor, calcule o frete para continuar.", ControlAppearance.Info);
                return;
            }

            var novoPedido = new Pedido
            {
                ClienteID = ClienteLogado.ClienteID,
                DataPedido = DateTime.Now,
                VendaValor = ValorTotalFinal
            };

            try
            {
                _pedidoService.CriarPedido(novoPedido, ItensDoCarrinho.ToList());
                MostrarSnackbar("Compra realizada com sucesso!", ControlAppearance.Success);

                _carrinhoService.LimparCarrinho(ClienteLogado.ClienteID);
                ItensDoCarrinho.Clear();
                Subtotal = 0;
                ValorFrete = 0;
                tbValorFrete.Text = "-";
                PainelResultados.Visibility = Visibility.Collapsed;
                txCep.Clear();
                OnPropertyChanged(nameof(ItensDoCarrinho));

                (sender as System.Windows.Controls.Button).IsEnabled = false;
            }
            catch (Exception ex)
            {
                MostrarSnackbar("Erro ao finalizar a compra. Tente novamente.", ControlAppearance.Danger);
                Console.WriteLine(ex.Message);
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
    }
}