using Pagamentos;

namespace Pagamentos
{
    public partial class ContaDetailsPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public ContaDetailsPage(int contaId)
        {
            InitializeComponent();

            // Inicializa o serviço de banco de dados
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "contas.db3");
            _databaseService = new DatabaseService(dbPath);

            // Carrega os detalhes da conta
            LoadContaDetails(contaId);
        }

        private async void LoadContaDetails(int contaId)
        {
            var conta = await _databaseService.GetContaByIdAsync(contaId);

            if (conta != null)
            {
                ContaNameLabel.Text = conta.Name;
                ContaStatusLabel.Text = conta.IsPaid ? "Paga" : "Não Paga";
                ContaDateLabel.Text = string.IsNullOrEmpty(conta.Date) ? "N/A" : conta.Date;
            }
            else
            {
                await DisplayAlert("Erro", "Conta não encontrada.", "OK");
                await Navigation.PopAsync();
            }
        }
    }
}
