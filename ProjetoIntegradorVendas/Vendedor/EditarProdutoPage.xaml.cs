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
    public partial class EditarProduoPage : Page
    {
        public Produto Produto { get; set; }
        private Fornecedor vendedorId;
        private string caminhoNovaImagem; // Armazena o caminho original da NOVA imagem selecionada

        public EditarProduoPage(Fornecedor vendedorId, Produto produto)
        {
            InitializeComponent();
            this.vendedorId = vendedorId;
            this.Produto = produto;
            this.DataContext = this;

            if (!string.IsNullOrEmpty(produto.Imagem))
            {
                txbNomeArquivo.Text = "Imagem atual: " + Path.GetFileName(produto.Imagem);
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
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Arquivos de Imagem (*.png;*.jpeg;*.jpg;*.webp)|*.png;*.jpeg;*.jpg;*.webp|Todos os arquivos (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                caminhoNovaImagem = openFileDialog.FileName;
                txbNomeArquivo.Text = "Nova imagem: " + Path.GetFileName(caminhoNovaImagem);
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

        private void SalvarProduto_Click(object sender, RoutedEventArgs e)
        {
            // Atualiza as propriedades do produto com os valores da tela
            Produto.Nome = txNomeProduto.Text;
            Produto.Descricao = txDescricaoProduto.Text;
            Produto.Preco = (double)(txPrecoProduto.Value ?? 0);
            Produto.Estoque = (int)(txEstoqueProduto.Value ?? 0);

            // Se uma nova imagem foi selecionada, processa ela
            if (!string.IsNullOrEmpty(caminhoNovaImagem))
            {
                string caminhoRelativoParaSalvar = CopiarImagemERetornarCaminhoRelativo(caminhoNovaImagem);
                if (caminhoRelativoParaSalvar == null)
                {
                    MostrarSnackbar("Ocorreu um erro ao processar a nova imagem.", ControlAppearance.Danger);
                    return;
                }
                // Atualiza a propriedade Imagem do produto com o NOVO caminho relativo
                Produto.Imagem = caminhoRelativoParaSalvar;
            }
            // Se nenhuma imagem nova foi selecionada, a propriedade Produto.Imagem manterá seu valor antigo.

            if (string.IsNullOrEmpty(Produto.Nome) || string.IsNullOrEmpty(Produto.Descricao))
            {
                MostrarSnackbar("Nome e Descrição são obrigatórios.", ControlAppearance.Danger);
                return;
            }

            var service = new ProdutoService();
            try
            {
                service.AtualizarProduto(Produto);
                MostrarSnackbar("Produto atualizado com sucesso!", ControlAppearance.Success);
            }
            catch (Exception ex)
            {
                MostrarSnackbar("Erro ao atualizar produto: " + ex.Message, ControlAppearance.Danger);
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
    }
}