using Pagamentos.Utils;

namespace Pagamentos
{
    public partial class RelatorioMensalPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        public RelatorioMensalPage()
        {
            InitializeComponent();

            // Inicializa o serviço de banco de dados
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "contas.db3");
            _databaseService = new DatabaseService(dbPath);

            // Preenche os pickers de mês e ano
            MesPicker.ItemsSource = new List<string>
            {
                "Janeiro", "Fevereiro", "Março", "Abril",
                "Maio", "Junho", "Julho", "Agosto",
                "Setembro", "Outubro", "Novembro", "Dezembro"
            };

            int anoAtual = DateTime.Now.Year;
            AnoPicker.ItemsSource = Enumerable.Range(anoAtual - 2, 3).Select(y => y.ToString()).ToList();
        }

        private async void OnGerarRelatorioClicked(object sender, EventArgs e)
        {
            if (MesPicker.SelectedIndex == -1 || AnoPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Erro", "Por favor, selecione o mês e o ano.", "OK");
                return;
            }

            string mesSelecionado = MesPicker.SelectedItem.ToString();
            int anoSelecionado = int.Parse(AnoPicker.SelectedItem.ToString());

            // Obtém as contas do banco de dados
            var contas = await _databaseService.GetContasAsync();
            var contasFiltradas = contas.Where(c =>
                DateTime.TryParse(c.DataVencimento, out DateTime dataVencimento) &&
                dataVencimento.Month == MesPicker.SelectedIndex + 1 &&
                dataVencimento.Year == anoSelecionado).ToList();

            // Calcula os totais
            int totalPagas = contasFiltradas.Count(c => c.IsPaid);
            int totalNaoPagas = contasFiltradas.Count(c => !c.IsPaid);
            decimal totalValorPagas = contasFiltradas.Where(c => c.IsPaid).Sum(c => decimal.Parse(c.Valor));
            decimal totalValorNaoPagas = contasFiltradas.Where(c => !c.IsPaid).Sum(c => decimal.Parse(c.Valor));

            // Atualiza o resumo
            // Atualiza o resumo
            ResumoLabel.Text = $"Pagas: {totalPagas}\n" +
                               $"Não Pagas: {totalNaoPagas}\n" +
                               $"Total Pago: {totalValorPagas:C}\n" +
                               $"Total Pendente: {totalValorNaoPagas:C}";

            // Atualiza a lista de contas
            ContasCollectionView.ItemsSource = contasFiltradas;
        }
    }
}
