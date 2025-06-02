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
using Wpf.Ui.Controls;
using Wpf.Ui;

namespace ProjetoIntegradorVendas
{
    /// <summary>  
    /// Interaction logic for LoginPage.xaml  
    /// </summary>  
    public partial class LoginPage : Page
    {

        public LoginPage()
        {
            InitializeComponent();
        }

        private void irParaCatalogo_Click(object sender, RoutedEventArgs e)
        {
            string sUsername = txUsuario.Text;
            string sPassword = txSenha.Password;

            string tipoLogin = (comboTipoLogin.SelectedItem as ComboBoxItem)?.Content.ToString();

            string tabela = tipoLogin == "Vendedor" ? "fornecedor" : "cliente";

            using (var conn = Database.GetConnection())
            {
                try
                {
                    conn.Open();

                    string query = $"SELECT COUNT(*) FROM {tabela} WHERE ClienteNome = @nome AND Senha = @senha";

                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nome", sUsername);
                        cmd.Parameters.AddWithValue("@senha", sPassword);

                        var count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            var mainWindow = (MainWindow)Application.Current.MainWindow;
                            mainWindow.MainFrame.Navigate(new CatalogoProdutosPage());
                        }
                        else
                        {
                            MostrarSnackbar("Usuário ou senha inválidos!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MostrarSnackbar("Erro ao conectar ao banco: " + ex.Message);
                }
            }
        }

        public void MostrarSnackbar(string mensagem)
        {
            Snackbar dlgMsg = new Snackbar(RootSnackbarPresenter);
            dlgMsg.Appearance = ControlAppearance.Danger;
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
    }
}

