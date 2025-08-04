using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ProjetoIntegradorVendas.Services;
using ProjetoIntegradorVendas.Vendedor;
using Wpf.Ui.Controls;
// Removido o using que causava conflito com TextBlock

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

            // Pré-popula os campos de texto com os valores do produto
            txNomeProduto.Text = produto.Nome;
            txDescricaoProduto.Text = produto.Descricao;
            txPrecoProduto.Text = produto.Preco.ToString(CultureInfo.InvariantCulture); // Converte número para texto
            txEstoqueProduto.Text = produto.Estoque.ToString(); // Converte número para texto

            if (!string.IsNullOrEmpty(produto.Imagem))
            {
                txbNomeArquivo.Text = "Imagem atual: " + Path.GetFileName(produto.Imagem);
            }
            else
            {
                txbNomeArquivo.Text = "Nenhuma imagem selecionada.";
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
            // Validação do Preço
            if (!double.TryParse(txPrecoProduto.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double preco))
            {
                MostrarSnackbar("O preço digitado não é um número válido.", ControlAppearance.Danger);
                return;
            }

            // Validação do Estoque
            if (!int.TryParse(txEstoqueProduto.Text, out int estoque))
            {
                MostrarSnackbar("A quantidade em estoque não é um número inteiro válido.", ControlAppearance.Danger);
                return;
            }

            // Atualiza as propriedades do produto com os valores da tela
            Produto.Nome = txNomeProduto.Text;
            Produto.Descricao = txDescricaoProduto.Text;
            Produto.Preco = preco; // Usa o valor convertido
            Produto.Estoque = estoque; // Usa o valor convertido

            if (!string.IsNullOrEmpty(caminhoNovaImagem))
            {
                string caminhoRelativoParaSalvar = CopiarImagemERetornarCaminhoRelativo(caminhoNovaImagem);
                if (caminhoRelativoParaSalvar == null)
                {
                    MostrarSnackbar("Ocorreu um erro ao processar a nova imagem.", ControlAppearance.Danger);
                    return;
                }
                Produto.Imagem = caminhoRelativoParaSalvar;
            }

            if (string.IsNullOrEmpty(Produto.Nome) || string.IsNullOrEmpty(Produto.Descricao))
            {
                MostrarSnackbar("Nome e Descrição são obrigatórios.", ControlAppearance.Danger);
                return;
            }

            var service = new ProdutoService();
            try
            {
                // Agora você precisa de um método no seu ProdutoService para ATUALIZAR
                // Supondo que você crie um método chamado "AtualizarProduto"
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
            // Usando System.Windows.Controls.TextBlock para evitar qualquer ambiguidade
            var snackbarTitle = new System.Windows.Controls.TextBlock
            {
                Text = mensagem,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                FontWeight = FontWeights.SemiBold
            };

            Snackbar dlgMsg = new Snackbar(RootSnackbarPresenter)
            {
                Appearance = aparencia,
                Title = snackbarTitle,
                IsCloseButtonEnabled = false
            };
            dlgMsg.Show();
        }

    }
}