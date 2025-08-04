using ProjetoIntegradorVendas.Classes;
using ProjetoIntegradorVendas.ClientePag;
using ProjetoIntegradorVendas.Services;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls;
using Wpf.Ui.Input;
namespace ProjetoIntegradorVendas.Cliente
{
    public partial class CarrinhoControl : UserControl
    {
        public ICommand RemoverItemCommand { get; }
        public ICommand ComprarCommand { get; }

        private readonly Classes.Cliente _clienteLogado;

        public CarrinhoControl(Classes.Cliente cliente)
        {
            InitializeComponent();
            RemoverItemCommand = new RelayCommand<object>(ExecutarRemoverItem);
            ComprarCommand = new RelayCommand<object>(ExecutarComprarAgora);
            this._clienteLogado = cliente;
            this.DataContext = this;
        }

        private void ExecutarRemoverItem(object parameter)
        {
            if (parameter is CarrinhoItem itemParaRemover)
            {
                var carrinhoService = new CarrinhoService();
                carrinhoService.RemoverDoCarrinho(itemParaRemover.CarrinhoItemID);

                if (CartItemsListView.ItemsSource is ObservableCollection<CarrinhoItem> colecao)
                {
                    colecao.Remove(itemParaRemover);
                }

                AtualizarTotal();
            }
        }

        public void AtualizarTotal()
        {
            if (CartItemsListView.ItemsSource is ObservableCollection<CarrinhoItem> colecao)
            {
                decimal valorTotal = colecao.Sum(item => (decimal)item.Produto.Preco * item.Quantidade);
                TotalCarrinho.Text = $"Total: {valorTotal:C}";
            }
        }

        private void ExecutarComprarAgora(object parameter)
        {
            try
            {

                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.MainFrame.Navigate(new ConfirmarCompraPage(_clienteLogado));
                mainWindow.CartFlyout.IsOpen = false;
            }
            catch (Exception ex)
            {
            }
        }
    }
}