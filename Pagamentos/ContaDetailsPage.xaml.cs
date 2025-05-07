using Pagamentos.Utils;
using OneSignalSDK.DotNet;

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
                ContaStatusLabel.TextColor = _conta.IsPaid ? Colors.Green : Colors.Red; // Define a cor com base no status
                ContaDateLabel.Text = string.IsNullOrEmpty(_conta.Date) ? "N/A" : _conta.Date;
                AvisarVencimentoCheckBox.IsChecked = _conta.AvisarVencimento;

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

                // Atualiza o campo AvisarVencimento com o estado do CheckBox
                _conta.AvisarVencimento = AvisarVencimentoCheckBox.IsChecked;

                // Salva as altera��es no banco de dados
                await _databaseService.SaveContaAsync(_conta);

                // Agendar notifica��o no OneSignal se AvisarVencimento estiver marcado
                if (_conta.AvisarVencimento)
                {
                    // Verifica se a data de vencimento � v�lida
                    if (DateTime.TryParse(_conta.DataVencimento, out DateTime dataVencimento))
                    {
                        // Verifica se a data atual � maior que a data de vencimento
                        if (DateTime.Now > dataVencimento)
                        {
                            await DisplayAlert("Aviso", "A data de vencimento j� passou. N�o � poss�vel agendar uma notifica��o.", "OK");
                            AvisarVencimentoCheckBox.IsChecked = false; // Desmarca o checkbox
                            return; // N�o gera o agendamento
                        }

                        var oneSignalService = new OneSignalService();

                        // Obt�m o idPlayer do banco de dados
                        var idPlayer = await _databaseService.GetPlayerIdAsync();
                        if (!string.IsNullOrEmpty(idPlayer))
                        {
                            // Define o t�tulo e o conte�do da notifica��o
                            string titulo = "Lembrete de Pagamento!";
                            string conteudo = $"A conta '{_conta.Name}' vence hoje! Valor: {_conta.Valor}";

                            // Converte a data para UTC
                            var dataEnvioUTC = dataVencimento.ToUniversalTime();

                            await oneSignalService.EnviarNotificacaoAsync(idPlayer, titulo, conteudo, dataEnvioUTC);
                        }
                        else
                        {
                            Console.WriteLine("[ERRO] ID do usu�rio n�o encontrado no banco de dados.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[ERRO] Data de vencimento inv�lida para a conta '{_conta.Name}'.");
                    }
                }

                await DisplayAlert("Sucesso", "Conta atualizada com sucesso!", "OK");

                await Navigation.PopAsync();
            }
        }

    }
}
