using System.Collections.Generic;

namespace Pagamentos
{
    public partial class HistoricoPage : ContentPage
    {
        public HistoricoPage(List<HistoricoConta> historico)
        {
            InitializeComponent();

            // Define os dados do histórico como o ItemsSource do ListView
            HistoricoListView.ItemsSource = historico;
        }
    }
}
