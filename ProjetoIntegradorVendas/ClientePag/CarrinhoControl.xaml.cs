using ProjetoIntegradorVendas.Classes;
using ProjetoIntegradorVendas.Services;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Input;
namespace ProjetoIntegradorVendas.Cliente
{
    public partial class CarrinhoControl : UserControl
    {
        public ICommand RemoverItemCommand { get; }

        public CarrinhoControl()
        {
            InitializeComponent();
            RemoverItemCommand = new RelayCommand<object>(ExecutarRemoverItem);
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
    }
}