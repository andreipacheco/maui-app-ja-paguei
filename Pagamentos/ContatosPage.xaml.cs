using Pagamentos.Models;
using Pagamentos.Utils;
using System.Collections.ObjectModel;

namespace Pagamentos
{
    public partial class ContatosPage : ContentPage
    {
        private DatabaseService _databaseService;
        private ObservableCollection<Contato> _contatos;

        public ContatosPage()
        {
            InitializeComponent();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "pagamentos.db");
            _databaseService = new DatabaseService(dbPath);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CarregarContatos();
        }

        private async Task CarregarContatos()
        {
            var contatos = await _databaseService.GetContatosAsync();
            _contatos = new ObservableCollection<Contato>(contatos);
            ListaContatos.ItemsSource = _contatos;
        }

        private async void OnAdicionarContatoClicked(object sender, EventArgs e)
        {
            var nome = NomeEntry.Text?.Trim();
            var chavePix = ChavePixEntry.Text?.Trim();

            if (string.IsNullOrEmpty(nome))
            {
                await DisplayAlert("Erro", "Por favor, informe o nome do contato.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(chavePix))
            {
                await DisplayAlert("Erro", "Por favor, informe a chave PIX.", "OK");
                return;
            }

            var novoContato = new Contato
            {
                Nome = nome,
                ChavePix = chavePix
            };

            await _databaseService.SaveContatoAsync(novoContato);

            // Limpar campos
            NomeEntry.Text = string.Empty;
            ChavePixEntry.Text = string.Empty;

            // Recarregar lista
            await CarregarContatos();

            await DisplayAlert("Sucesso", "Contato adicionado com sucesso!", "OK");
        }

        private async void OnCopiarPixClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Contato contato)
            {
                await Clipboard.SetTextAsync(contato.ChavePix);
                await DisplayAlert("Copiado", $"Chave PIX de {contato.Nome} copiada para a área de transferência!", "OK");
            }
        }

        private async void OnDeleteContatoSwipeInvoked(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is Contato contato)
            {
                bool confirmacao = await DisplayAlert(
                    "Confirmar Exclusão",
                    $"Deseja realmente excluir o contato {contato.Nome}?",
                    "Sim",
                    "Não");

                if (confirmacao)
                {
                    await _databaseService.DeleteContatoAsync(contato);
                    await CarregarContatos();
                    await DisplayAlert("Sucesso", "Contato excluído com sucesso!", "OK");
                }
            }
        }
    }
}
