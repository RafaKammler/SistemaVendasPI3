using System;
using System.Windows;
using System.Windows.Controls;
using ProjetoIntegradorVendas.Classes;
using ProjetoIntegradorVendas.Services;
using Wpf.Ui.Controls;
using TextBlock = System.Windows.Controls.TextBlock;

namespace ProjetoIntegradorVendas
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void irParaCatalogo_Click(object sender, RoutedEventArgs e)
        {
            string username = txUsuario.Text;
            string password = txSenha.Password;
            string tipoLogin = (comboTipoLogin.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(tipoLogin))
            {
                MostrarSnackbar("Por favor, selecione o tipo de login.");
                return;
            }

            try
            {
                if (tipoLogin == "Cliente")
                {
                    AutenticarCliente(username, password);
                }
                else if (tipoLogin == "Vendedor")
                {
                    AutenticarVendedor(username, password);
                }
            }
            catch (Exception ex)
            {
                MostrarSnackbar("Erro de conexão: " + ex.Message);
            }
        }

        private void AutenticarCliente(string username, string password)
        {
            var clienteService = new ClienteService();
            Classes.Cliente clienteLogado = clienteService.Autenticar(username, password);

            if (clienteLogado != null)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.MainFrame.Navigate(new CatalogoProdutosPage(clienteLogado));
            }
            else
            {
                MostrarSnackbar("Cliente ou senha inválidos!");
            }
        }

        private void AutenticarVendedor(string username, string password)
        {
            var fornecedorService = new FornecedorService();
            Fornecedor fornecedorLogado = fornecedorService.Autenticar(username, password);

            if (fornecedorLogado != null)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.MainFrame.Navigate(new Vendedor.CatalogoVendedorPage(fornecedorLogado));
            }
            else
            {
                MostrarSnackbar("Vendedor ou senha inválidos!");
            }
        }

        public void MostrarSnackbar(string mensagem)
        {
            Snackbar dlgMsg = new Snackbar(RootSnackbarPresenter);
            dlgMsg.Appearance = ControlAppearance.Danger;
            dlgMsg.Title = new TextBlock
            {
                Text = mensagem,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                FontWeight = FontWeights.SemiBold
            };
            dlgMsg.IsCloseButtonEnabled = false;
            dlgMsg.Show();
        }
    }
}
