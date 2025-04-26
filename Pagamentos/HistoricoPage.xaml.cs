using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pagamentos
{
    public partial class HistoricoPage : ContentPage
    {
        private DatabaseService _databaseService;
        public ObservableCollection<HistoricoConta> HistoricoContas { get; set; } = new ObservableCollection<HistoricoConta>();

        public HistoricoPage(List<HistoricoConta> historico)
        {
            InitializeComponent();

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "contas.db3");
            _databaseService = new DatabaseService(dbPath);

            BindingContext = this;

            LoadHistoricoFromDb();
        }

        private async void LoadHistoricoFromDb()
        {
            var historicos = await _databaseService.GetHistoricosAsync();

            var historicosOrdenados = historicos
                .OrderByDescending(h => h.Date)
                .ToList();

            HistoricoContas.Clear();

            foreach (var item in historicosOrdenados)
            {
                HistoricoContas.Add(item);
            }
        }

    }
}

