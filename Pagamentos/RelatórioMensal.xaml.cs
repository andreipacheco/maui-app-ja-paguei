using Pagamentos.Utils;
using System.Globalization;

namespace Pagamentos
{
    public partial class RelatorioMensalPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly CultureInfo _cultureInfo = new CultureInfo("pt-BR");

        public RelatorioMensalPage()
        {
            InitializeComponent();

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "contas.db3");
            _databaseService = new DatabaseService(dbPath);

            MesPicker.ItemsSource = new List<string>
            {
                "Janeiro", "Fevereiro", "Março", "Abril",
                "Maio", "Junho", "Julho", "Agosto",
                "Setembro", "Outubro", "Novembro", "Dezembro"
            };

            int anoAtual = DateTime.Now.Year;
            AnoPicker.ItemsSource = Enumerable.Range(anoAtual - 2, 5).Select(y => y.ToString()).ToList();
        }

        private async void OnGerarRelatorioClicked(object sender, EventArgs e)
        {
            if (MesPicker.SelectedIndex == -1 || AnoPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Erro", "Por favor, selecione o mês e o ano.", "OK");
                return;
            }

            try
            {
                int mesSelecionado = MesPicker.SelectedIndex + 1;
                int anoSelecionado = int.Parse(AnoPicker.SelectedItem.ToString());

                // Obtém o histórico do banco de dados
                var historicos = await _databaseService.GetHistoricosAsync();

                // Processa os registros
                var registrosProcessados = historicos
                    .Where(h => h.Date.Month == mesSelecionado && h.Date.Year == anoSelecionado)
                    .Select(h => {
                        decimal valor = ConverterValor(h.Valor);
                        bool pago = valor > 0;

                        return new
                        {
                            Nome = h.Name,
                            Data = h.Date,
                            Valor = valor,
                            Pago = pago
                        };
                    })
                    .ToList();

                // Calcula os totais
                int totalPagas = registrosProcessados.Count(h => h.Pago);
                int totalNaoPagas = registrosProcessados.Count(h => !h.Pago);
                decimal totalValorPagas = registrosProcessados.Where(h => h.Pago).Sum(h => h.Valor);
                decimal totalValorNaoPagas = registrosProcessados.Where(h => !h.Pago).Sum(h => h.Valor);

                // Atualiza o resumo
                ResumoLabel.Text = $"Total Pago: {totalValorPagas.ToString("C", _cultureInfo)}";
                                   

                // Atualiza a lista de histórico
                ContasCollectionView.ItemsSource = registrosProcessados
                    .OrderBy(h => h.Data)
                    .Select(h => new
                    {
                        Name = h.Nome,
                        Data = h.Data.ToString("dd/MM/yyyy"),
                        Valor = h.Valor.ToString("N2", _cultureInfo)
                    });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Ocorreu um erro ao gerar o relatório: {ex.Message}", "OK");
            }
        }

        private decimal ConverterValor(string valorStr)
        {
            if (string.IsNullOrWhiteSpace(valorStr))
                return 0m;

            // Remove caracteres não numéricos exceto vírgula/ponto
            valorStr = new string(valorStr.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());

            // Verifica se tem vírgula como separador decimal
            if (valorStr.Contains(','))
            {
                // Remove pontos de milhar e troca vírgula por ponto para o parse
                valorStr = valorStr.Replace(".", "").Replace(",", ".");
            }

            if (decimal.TryParse(valorStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
            {
                return valor;
            }
            return 0m;
        }
    }
}