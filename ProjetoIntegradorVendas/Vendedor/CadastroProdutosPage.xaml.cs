using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ProjetoIntegradorVendas.Services;
using ProjetoIntegradorVendas.Vendedor;
using Wpf.Ui.Controls;
using TextBlock = Wpf.Ui.Controls.TextBlock;

namespace ProjetoIntegradorVendas
{
    public partial class CadastroProdutosPage : Page
    {
        private Fornecedor vendedorId;
        private string caminhoImagemOriginal;

        public CadastroProdutosPage(Fornecedor vendedorId)
        {
            InitializeComponent();
            this.vendedorId = vendedorId;
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

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivos de Imagem (*.png;*.jpeg;*.jpg;*.webp)|*.png;*.jpeg;*.jpg;*.webp|Todos os arquivos (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                caminhoImagemOriginal = openFileDialog.FileName;
                txbNomeArquivo.Text = "Imagem selecionada: " + Path.GetFileName(caminhoImagemOriginal);
            }
        }

        private string CopiarImagemERetornarCaminhoRelativo(string caminhoOrigem)
        {
            try
            {
                string pastaDestino = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "images");
                Directory.CreateDirectory(pastaDestino);

                string extensao = Path.GetExtension(caminhoOrigem);
                string nomeArquivoUnico = Guid.NewGuid().ToString() + extensao;

                string caminhoDestinoCompleto = Path.Combine(pastaDestino, nomeArquivoUnico);
                File.Copy(caminhoOrigem, caminhoDestinoCompleto);

                return Path.Combine("/resources", "images", nomeArquivoUnico).Replace('\\', '/');
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao copiar arquivo: " + ex.Message);
                return null;
            }
        }

        private void CadastrarProduto_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(caminhoImagemOriginal))
            {
                MostrarSnackbar("Por favor, selecione uma imagem para o produto.", ControlAppearance.Danger);
                return;
            }

            string caminhoRelativoParaSalvar = CopiarImagemERetornarCaminhoRelativo(caminhoImagemOriginal);

            if (caminhoRelativoParaSalvar == null)
            {
                MostrarSnackbar("Ocorreu um erro ao processar a imagem.", ControlAppearance.Danger);
                return;
            }

            string nomeProduto = txNomeProduto.Text;
            string descricaoProduto = txDescricaoProduto.Text;

            if (!Double.TryParse(txPrecoProduto.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double preco))
            {
                MostrarSnackbar("O preço do produto deve ser um número válido.", ControlAppearance.Danger);
                return;
            }

            if (!int.TryParse(txEstoqueProduto.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out int estoque))
            {
                MostrarSnackbar("O estoque deve ser um número inteiro.", ControlAppearance.Danger);
                return;
            }

            var fornecedorService = new FornecedorService();
            Fornecedor fornecedor = fornecedorService.encontrarFornecedor(vendedorId);

            var produto = new Produto
            {
                IdFornecedor = fornecedor,
                Nome = nomeProduto,
                Descricao = descricaoProduto,
                Preco = preco,
                Imagem = caminhoRelativoParaSalvar,
                Estoque = estoque,
                ImagemPath = null
            };

            var service = new ProdutoService();
            try
            {
                service.CadastrarProduto(produto);
                MostrarSnackbar("Produto cadastrado com sucesso.", ControlAppearance.Success);

                txNomeProduto.Clear();
                txDescricaoProduto.Clear();
                txPrecoProduto.Clear();
                txEstoqueProduto.Clear();
                caminhoImagemOriginal = string.Empty;
                txbNomeArquivo.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MostrarSnackbar("Erro ao cadastrar produto: " + ex.Message, ControlAppearance.Danger);
            }
        }

        public void MostrarSnackbar(string mensagem, ControlAppearance aparencia)
        {
            Snackbar dlgMsg = new Snackbar(RootSnackbarPresenter)
            {
                Appearance = aparencia,
                Title = new TextBlock
                {
                    Text = mensagem,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    FontWeight = FontWeights.SemiBold
                },
                IsCloseButtonEnabled = false
            };
            dlgMsg.Show();
        }

        private void NumberBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}