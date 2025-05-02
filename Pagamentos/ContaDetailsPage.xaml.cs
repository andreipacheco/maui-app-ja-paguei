using Pagamentos.Utils;

namespace Pagamentos
{
    public partial class ContaDetailsPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private Conta _conta;

        public ContaDetailsPage(int contaId)
        {
            InitializeComponent();

            // Inicializa o servi�o de banco de dados
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "contas.db3");
            _databaseService = new DatabaseService(dbPath);

            // Carrega os detalhes da conta
            LoadContaDetails(contaId);
        }

        private async void LoadContaDetails(int contaId)
        {
            _conta = await _databaseService.GetContaByIdAsync(contaId);

            if (_conta != null)
            {
                ContaNameLabel.Text = _conta.Name;
                ContaStatusLabel.Text = _conta.IsPaid ? "Paga" : "N�o Paga";
                ContaDateLabel.Text = string.IsNullOrEmpty(_conta.Date) ? "N/A" : _conta.Date;

                // Preenche os campos opcionais
                ValorEntry.Text = _conta.Valor;
                if (!string.IsNullOrEmpty(_conta.DataVencimento))
                {
                    DataVencimentoPicker.Date = DateTime.Parse(_conta.DataVencimento);
                }
            }
            else
            {
                await DisplayAlert("Erro", "Conta n�o encontrada.", "OK");
                await Navigation.PopAsync();
            }
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (_conta != null)
            {
                // Atualiza os campos opcionais
                if (!string.IsNullOrWhiteSpace(ValorEntry.Text))
                {
                    _conta.Valor = ValorEntry.Text; // Atribui o valor diretamente como string
                }
                else
                {
                    _conta.Valor = "0,00"; // Valor padr�o caso a entrada seja inv�lida ou vazia
                }

                _conta.DataVencimento = DataVencimentoPicker.Date.ToString("dd/MM/yyyy");

                // Salva as altera��es no banco de dados
                await _databaseService.SaveContaAsync(_conta);

                await DisplayAlert("Sucesso", "Conta atualizada com sucesso!", "OK");
               
                await Navigation.PopAsync();
            }
        }
    }
}
