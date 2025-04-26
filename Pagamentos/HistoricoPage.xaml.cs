using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pagamentos
{
    public partial class HistoricoPage : ContentPage
    {
        private DatabaseService _databaseService;
        private ObservableCollection<HistoricoConta> historicoContas = new ObservableCollection<HistoricoConta>();
        public HistoricoPage(List<HistoricoConta> historico)
        {
            InitializeComponent();

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "contas.db3");
            _databaseService = new DatabaseService(dbPath);

            LoadHistoricoFromDb();
        }

        private async void LoadHistoricoFromDb()
        {
            var historicos = await _databaseService.GetHistoricosAsync();
            foreach (var item in historicos)
            {
                historicoContas.Add(item);
            }
        }
    }
}

