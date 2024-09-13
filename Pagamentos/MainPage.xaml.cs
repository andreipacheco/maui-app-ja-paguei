
using System.Collections.ObjectModel;
using System.Text.Json;


namespace Pagamentos
{
    public partial class MainPage : ContentPage
    {
        ObservableCollection<Conta> contas = new ObservableCollection<Conta>();
        public ObservableCollection<Conta> Contas { get { return contas; } }

        public MainPage()
        {
            InitializeComponent();

            // Carrega o mês de referência salvo
            LoadMesReferencia();

            // Carrega as contas do Preferences
            LoadContas();

            // Configura o ItemsSource do ListView
            ListaContas.ItemsSource = contas;
        }

        private void AddClicked(object sender, EventArgs e)
        {
            // Adiciona nova conta à lista
            contas.Add(new Conta { Name = NovaConta.Text, IsPaid = false, Date = "" });

            // Limpa campo da Entry
            NovaConta.Text = "";

            // Salva as contas no Preferences
            SaveContas();
        }

        private void OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var conta = checkbox.BindingContext as Conta;

            if (conta != null && conta.IsPaid) // Se o item foi marcado como pago
            {
                // Persistir a data atual no campo "Date" quando for pago
                conta.Date = DateTime.Now.ToString("dd/MM/yyyy");

                // Salva as contas atualizadas no Preferences
                SaveContas();
            }
        }

        private void OnContaTapped(object sender, ItemTappedEventArgs e)
        {
            var contaSelecionada = e.Item as Conta;
            if (contaSelecionada != null && !string.IsNullOrEmpty(contaSelecionada.Date))
            {
                // Mostra a data de pagamento da conta em um DisplayAlert
                DisplayAlert("Informação de Pagamento", $"Conta paga em: {contaSelecionada.Date}", "OK");
            }
            else
            {
                DisplayAlert("Atenção!", "Essa conta ainda não foi paga!", "OK");
            }

            // Desmarca o item após a seleção para evitar que fique selecionado visualmente
            ((ListView)sender).SelectedItem = null;
        }

        private void SaveContas()
        {
            var contasJson = JsonSerializer.Serialize(contas);
            Preferences.Set("contas", contasJson);
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

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            var contaSelecionada = sender as Conta;
            // var contaSelecionada = e.Item as Conta;
            Console.WriteLine("contaSelecionada");
            Console.WriteLine(e);
            if (contaSelecionada != null)
            {
                // Confirmação antes de deletar
                bool confirm = DisplayAlert("Remover Conta", $"Deseja remover a conta \"{contaSelecionada.Name}\"?", "Sim", "Não").Result;

                if (confirm)
                {
                    // Remove a conta da ObservableCollection
                    contas.Remove(contaSelecionada);

                    // Atualiza as contas no Preferences
                    SaveContas();

                    // Alerta de sucesso
                    DisplayAlert("Menos uma!!", "Conta deletada com sucesso!", "OK");
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
    }
}
