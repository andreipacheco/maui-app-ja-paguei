
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using OneSignalSDK.DotNet;
using Pagamentos;
using System.Collections.ObjectModel;
using System.Text;
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

            BindingContext = this;
            // Configura o ItemsSource do ListView
            ListaContas.ItemsSource = contas;

            picker.ItemsSource = new List<string>
            {
                "Janeiro", "Fevereiro", "Março", "Abril",
                "Maio", "Junho", "Julho", "Agosto",
                "Setembro", "Outubro", "Novembro", "Dezembro"
            };

            // Salva a assinatura de push no SQLite
            SavePushSubscriptionAsync();

            // ScheduleMonthlyNotification();

            // Adiciona um atraso de 5 segundos antes de chamar ScheduleTestNotification, para dar tempo de salvar o idPush
            //Task.Run(async () =>
            //{
            //    await Task.Delay(5000); // 5 segundos
            //    await ScheduleTestNotification();
            //});
        }


        private async Task SavePushSubscriptionAsync()
        {
            try
            {
                // Obtém a assinatura de push do OneSignal
                var push = OneSignal.User.PushSubscription;

                if (push != null && !string.IsNullOrEmpty(push.Id))
                {
                    // Salva o Player ID no SQLite
                    await _databaseService.SavePlayerIdAsync(push.Id);
                    Console.WriteLine($"PushSubscription salvo com sucesso: {push.Id}");
                }
                else
                {
                    Console.WriteLine("PushSubscription não encontrado ou inválido.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar PushSubscription: {ex.Message}");
            }
        }

        private async Task ScheduleMonthlyNotification()
        {
            try
            {
                var oneSignal = new OneSignalService();

                // Verifica se hoje é dia 1
                if (DateTime.Today.Day == 1)
                {
                    // Obtém o idPlayer do banco de dados
                    var idPlayer = await _databaseService.GetPlayerIdAsync();

                    if (!string.IsNullOrEmpty(idPlayer))
                    {
                        // Envia a notificação com o idPlayer
                        await oneSignal.EnviarNotificacaoAsync(idPlayer);
                        Console.WriteLine("Notificação enviada com sucesso para o idPlayer.");
                    }
                    else
                    {
                        Console.WriteLine("idPlayer não encontrado no banco de dados. Notificação não enviada.");
                    }
                }
                else
                {
                    Console.WriteLine("Hoje não é dia 1. Notificação não enviada.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao agendar notificação mensal: {ex.Message}");
            }
        }

        private async Task ScheduleTestNotification()
        {
            var oneSignal = new OneSignalService();
            var idPlayer = await _databaseService.GetPlayerIdAsync(); // Obtém o idPlayer do SQLite

            if (!string.IsNullOrEmpty(idPlayer))
            {
                await oneSignal.EnviarNotificacaoAsync(idPlayer); // Passa o idPlayer como parâmetro
            }
            else
            {
                Console.WriteLine("idPlayer não encontrado no banco de dados.");
            }
        }

        private async void OnPaySwipeInvoked(object sender, EventArgs e)
            {
                if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is Conta conta)
                {
                    conta.IsPaid = !conta.IsPaid;

                    // Atualiza a data de pagamento se a conta for marcada como paga
                    if (conta.IsPaid)
                    {
                        conta.Date = DateTime.Now.ToString("dd/MM/yyyy");

                        // Adiciona no histórico
                        var historico = new HistoricoConta
                        {
                            Name = conta.Name,
                            Date = DateTime.Parse(conta.Date)  // Converte a string para DateTime
                        };
                        await _databaseService.SaveHistoricoAsync(historico);
                    }
                    else
                    {
                        conta.Date = string.Empty;
                    }

                    // Salva a conta no banco de dados (chamada assíncrona)
                    await SaveConta(conta);

                    // Atualiza a coleção de contas
                    // Não é necessário redefinir o ItemsSource, já que a ObservableCollection faz isso automaticamente
                    // O ListView irá automaticamente atualizar a interface para refletir a mudança no estado da conta

                    // Exibe a mensagem de sucesso
                    // await DisplayAlert("Sucesso", $"{conta.Name} foi marcada como paga!", "OK");
                    await Toast.Make($"{conta.Name} foi marcada como paga!",
                     ToastDuration.Long)
                       .Show();
                }
        }

        private async void OnDeleteSwipeInvoked(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is Conta conta)
            {
                bool confirm = await DisplayAlert("Confirmar", $"Deseja remover a conta '{conta.Name}'?", "Sim", "Não");

                if (confirm)
                {
                    // Remove da ObservableCollection
                    contas.Remove(conta);

                    // Deleta do banco de dados
                    await _databaseService.DeleteContaAsync(conta);

                    // Exibe a mensagem de sucesso
                    // await DisplayAlert("Sucesso", $"Conta '{conta.Name}' deletada com sucesso!", "OK");
                    await Toast.Make($"Conta '{conta.Name}' deletada com sucesso!",
                  ToastDuration.Long)
                    .Show();
                }
            }
        }

        private async void OnEditSwipeInvoked(object sender, EventArgs e)
        {
            // TODO: Implementar a funcionalidade de edição
            if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is Conta conta)
            {
                // Exibe um campo de entrada para editar o nome da conta
                string result = await DisplayPromptAsync("Editar Conta", "Digite o novo nome da conta:", initialValue: conta.Name);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    conta.Name = result;
                    // Salva a conta no banco de dados
                    await SaveConta(conta);
                    // Atualiza a ObservableCollection
                    // Não é necessário redefinir o ItemsSource, já que a ObservableCollection faz isso automaticamente
                }
            }
        }

        private async void AddClicked(object sender, EventArgs e)
        {
            // Verifica se o campo está vazio ou tem menos de 5 caracteres
            if (string.IsNullOrWhiteSpace(NovaConta.Text) || NovaConta.Text.Length < 3)
            {
                // Exibe um aviso ao usuário
                await Toast.Make("O nome da conta deve ter no mínimo 3 caracteres e não pode ser vazio",
                  ToastDuration.Long)
                    .Show();
                return;
            }

            // Cria uma nova conta e adiciona à coleção
            var novaConta = new Conta { Name = NovaConta.Text, IsPaid = false, Date = "" };
            contas.Add(novaConta);

            // Salva a conta no banco de dados
            await SaveConta(novaConta);

            // Limpa o campo de entrada
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
                        Date = DateTime.Parse(conta.Date)  // Converte a string para DateTime
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

        //private void OnContaTapped(object sender, ItemTappedEventArgs e)
        //{
        //    var contaSelecionada = e.Item as Conta;
        //    if (contaSelecionada != null && !string.IsNullOrEmpty(contaSelecionada.Date))
        //    {
        //        // Mostra a data de pagamento da conta em um DisplayAlert
        //        DisplayAlert("Informação de Pagamento", $"{contaSelecionada.Name} paga em: {contaSelecionada.Date}", "OK");
        //    }
        //    else
        //    {
        //        DisplayAlert("Atenção!", "Essa conta ainda não foi paga!", "OK");
        //    }

        //    // Desmarca o item após a seleção para evitar que fique selecionado visualmente
        //    ((ListView)sender).SelectedItem = null;
        //}

        private async Task SaveConta(Conta conta)
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

        private async Task LoadMesReferencia()
        {
            var mesReferencia = await _databaseService.GetUltimoMesReferenciaAsync();
            if (mesReferencia != null)
            {
                picker.SelectedItem = mesReferencia.NomeMes;
            }
        }

        private async void salvarMesReferencia_Clicked(object sender, EventArgs e)
        {
            // Obtém o valor selecionado no Picker
            var mesSelecionado = picker.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(mesSelecionado))
            {
                var novoMes = new MesReferencia { NomeMes = mesSelecionado };
                await _databaseService.SaveMesReferenciaAsync(novoMes);

                // Atualiza todas as contas para IsPaid = false
                var todasContas = await _databaseService.GetContasAsync();
                foreach (var conta in todasContas)
                {
                    conta.IsPaid = false;
                    conta.Date = string.Empty; // Limpa a data de pagamento
                    await _databaseService.SaveContaAsync(conta);
                }

                // Atualiza a ObservableCollection para refletir as mudanças na interface
                contas.Clear();
                foreach (var conta in todasContas)
                {
                    contas.Add(conta);
                }

                // Exibe uma mensagem de confirmação
                await DisplayAlert("Sucesso", $"Mês de referência '{mesSelecionado}' salvo com sucesso e todas as contas foram resetadas!", "OK");
            }
            else
            {
                // Exibe um alerta se nenhum mês foi selecionado
                await DisplayAlert("Atenção", "Por favor, selecione um mês de referência.", "OK");
            }
        }

        private async void salvarHistoricoMesReferencia_Clicked(object sender, EventArgs e)
        {
            // Verifica se todas as contas estão pagas
            var contasNaoPagas = contas.Where(c => !c.IsPaid).ToList();

            if (contasNaoPagas.Count > 0)
            {
                // Alerta o usuário que ainda há contas não pagas
                // await DisplayAlert("Atenção", "Ainda há contas não pagas. Por favor, quite todas as contas antes de salvar o histórico.", "OK");

                await Toast.Make("Ainda há contas não pagas. Quite todas as contas antes de salvar o histórico.",
                      ToastDuration.Long)
                        .Show();
                return;
            }

            // Se todas as contas foram pagas, salvar o histórico
            var mesSelecionado = picker.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(mesSelecionado))
            {
                // await DisplayAlert("Atenção", "Por favor, selecione um mês de referência.", "OK");
                await Toast.Make("Por favor, selecione um mês de referência.",
                     ToastDuration.Long)
                       .Show();
                return;
            }

            // Armazena o histórico de contas pagas com as respectivas datas
            var contasPagasHistorico = contas.Select(c => new HistoricoConta
            {
                Name = c.Name,
                Date = !string.IsNullOrEmpty(c.Date) ? DateTime.Parse(c.Date) : default
            }).ToList();

            // Salva cada item do histórico no banco de dados
            foreach (var historico in contasPagasHistorico)
            {
                await _databaseService.SaveHistoricoAsync(historico);
            }

            // Exibe uma mensagem de sucesso
            await DisplayAlert("Sucesso", $"Histórico do mês de '{mesSelecionado}' salvo com sucesso!", "OK");
        }

        private async void historicoMesReferencia_Clicked(object sender, EventArgs e)
        {
            // Obtém o mês selecionado no Picker
            var mesSelecionado = picker.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(mesSelecionado))
            {
                await DisplayAlert("Atenção", "Por favor, selecione um mês de referência.", "OK");
                return;
            }

            // Carrega o histórico do banco de dados
            var historico = await _databaseService.GetHistoricosAsync();


            // Filtra o histórico pelo mês selecionado
            var historicoFiltrado = historico.Where(h => h.Date.ToString("MMMM", new System.Globalization.CultureInfo("pt-BR")) == mesSelecionado).ToList();

            // Navega para a página de histórico e passa os dados
            await Navigation.PushAsync(new HistoricoPage(historicoFiltrado));
        }
    }
}
