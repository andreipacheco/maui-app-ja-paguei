
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

        // Evento disparado quando o valor do CheckBox é alterado
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

            // TODO: Após clicar em um item ele fica selecionado, ajustar.
        }

        // Método para salvar a lista de contas no Preferences
        private void SaveContas()
        {
            var contasJson = JsonSerializer.Serialize(contas);
            Preferences.Set("contas", contasJson);
        }

        // Método para carregar a lista de contas do Preferences
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
    }
}
