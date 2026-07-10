namespace Pagamentos
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registrar rotas de navegação
            Routing.RegisterRoute("ContatosPage", typeof(ContatosPage));
        }
    }
}
