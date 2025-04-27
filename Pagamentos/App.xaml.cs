using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;

namespace Pagamentos
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }
    }
}
