
using Pagamentos;
using System.Collections.ObjectModel;
using System.Text.Json;


namespace Pagamentos
{
    public partial class MainPage : ContentPage
    {
        ObservableCollection<Conta> contas = new ObservableCollection<Conta>();
        public ObservableCollection<Conta> Contas { get { return contas; } }
        private DatabaseService _databaseService;
        public MainPage()
        {
            InitializeComponent();

            // Inicializa o caminho do banco
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "contas.db3");
            _databaseService = new DatabaseService(dbPath);

            // Carrega dados
            LoadContasFromDb();

            // Carrega o mês
            LoadMesReferencia();

            // Configura o ItemsSource do ListView
            ListaContas.ItemsSource = contas;
        }

        private void AddClicked(object sender, EventArgs e)
        {
            var novaConta = new Conta { Name = NovaConta.Text, IsPaid = false, Date = "" };
            contas.Add(novaConta);
            SaveConta(novaConta);

            NovaConta.Text = "";
        }

        private async void OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var conta = checkbox.BindingContext as Conta;

            if (conta != null)
            {
                if (conta.IsPaid)
                {
                    conta.Date = DateTime.Now.ToString("dd/MM/yyyy");

                    // Adiciona no histórico
                    var historico = new HistoricoConta
                    {
                        Name = conta.Name,
                        Date = conta.Date
                    };
                    await _databaseService.SaveHistoricoAsync(historico);
                }
                else
                {
                    conta.Date = string.Empty;
                }

                SaveConta(conta);
            }
        }

        private void OnContaTapped(object sender, ItemTappedEventArgs e)
        {
            var contaSelecionada = e.Item as Conta;
            if (contaSelecionada != null && !string.IsNullOrEmpty(contaSelecionada.Date))
            {
                // Mostra a data de pagamento da conta em um DisplayAlert
                DisplayAlert("Informação de Pagamento", $"{contaSelecionada.Name} paga em: {contaSelecionada.Date}", "OK");
            }
            else
            {
                DisplayAlert("Atenção!", "Essa conta ainda não foi paga!", "OK");
            }

            // Desmarca o item após a seleção para evitar que fique selecionado visualmente
            ((ListView)sender).SelectedItem = null;
        }

        private async void SaveConta(Conta conta)
        {
            await _databaseService.SaveContaAsync(conta);
        }

        private async void LoadContasFromDb()
        {
            var contasList = await _databaseService.GetContasAsync();
            foreach (var conta in contasList)
            {
                contas.Add(conta);
            }
        }

        private void LoadContas()
        {
            var contasJson = Preferences.Get("contas", string.Empty);
            if (!string.IsNullOrEmpty(contasJson))
            {
                var contasList = JsonSerializer.Deserialize<List<Conta>>(contasJson);

                // Recarrega as contas na ObservableCollection
                foreach (var conta in contasList)
                {
                    contas.Add(conta);
                }
            }
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var contaSelecionada = button.BindingContext as Conta;

            if (contaSelecionada != null)
            {
                bool confirm = await DisplayAlert("Remover Conta", $"Deseja remover a conta \"{contaSelecionada.Name}\"?", "Sim", "Não");

                if (confirm)
                {
                    contas.Remove(contaSelecionada);
                    await _databaseService.DeleteContaAsync(contaSelecionada);

                    await DisplayAlert("Sucesso", "Conta deletada com sucesso!", "OK");
                }
            }
        }

        private void LoadMesReferencia()
        {
            // Recupera o mês de referência salvo no Preferences
            var mesSalvo = Preferences.Get("mesReferencia", string.Empty);

            if (!string.IsNullOrEmpty(mesSalvo))
            {
                // Define o mês salvo como o item selecionado no Picker
                picker.SelectedItem = mesSalvo;
            }
        }

        private void salvarMesReferencia_Clicked(object sender, EventArgs e)
        {
            // Obtém o valor selecionado no Picker
            var mesSelecionado = picker.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(mesSelecionado))
            {
                // Persiste o mês selecionado no Preferences
                Preferences.Set("mesReferencia", mesSelecionado);

                // Exibe uma mensagem de confirmação
                DisplayAlert("Sucesso", $"Mês de referência '{mesSelecionado}' salvo com sucesso!", "OK");
            }
            else
            {
                // Exibe um alerta se nenhum mês foi selecionado
                DisplayAlert("Atenção", "Por favor, selecione um mês de referência.", "OK");
            }
        }

        private void salvarHistoricoMesReferencia_Clicked(object sender, EventArgs e)
        {
            // Verifica se todas as contas estão pagas
            var contasNaoPagas = contas.Where(c => !c.IsPaid).ToList();

            if (contasNaoPagas.Count > 0)
            {
                // Alerta o usuário que ainda há contas não pagas
                DisplayAlert("Atenção", "Ainda há contas não pagas. Por favor, quite todas as contas antes de salvar o histórico.", "OK");
                return;
            }

            // Se todas as contas foram pagas, salvar o histórico
            var mesSelecionado = picker.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(mesSelecionado))
            {
                DisplayAlert("Atenção", "Por favor, selecione um mês de referência.", "OK");
                return;
            }

            // Armazena o histórico de contas pagas com as respectivas datas usando a nova classe HistoricoConta
            var contasPagasHistorico = contas.Select(c => new HistoricoConta { Name = c.Name, Date = c.Date }).ToList();

            // Serializa o histórico para salvar no Preferences
            var historicoJson = JsonSerializer.Serialize(contasPagasHistorico);

            // Salva o histórico com o nome do mês como chave no Preferences
            Preferences.Set($"historico_{mesSelecionado}", historicoJson);

            // Exibe uma mensagem de sucesso
            DisplayAlert("Sucesso", $"Histórico do mês de '{mesSelecionado}' salvo com sucesso!", "OK");
        }

        private void historicoMesReferencia_Clicked(object sender, EventArgs e)
        {
            // Obtém o mês selecionado no Picker
            var mesSelecionado = picker.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(mesSelecionado))
            {
                DisplayAlert("Atenção", "Por favor, selecione um mês de referência.", "OK");
                return;
            }

            // Carrega o histórico do mês selecionado do Preferences
            var historicoJson = Preferences.Get($"historico_{mesSelecionado}", string.Empty);

            if (string.IsNullOrEmpty(historicoJson))
            {
                DisplayAlert("Atenção", $"Não há histórico salvo para o mês de '{mesSelecionado}'.", "OK");
                return;
            }

            // Desserializa o histórico
            var historico = JsonSerializer.Deserialize<List<HistoricoConta>>(historicoJson);

            // Navega para a página de histórico e passa os dados
            Navigation.PushAsync(new HistoricoPage(historico));
        }
    }
}
