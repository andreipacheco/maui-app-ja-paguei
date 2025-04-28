using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using OneSignalSDK.DotNet;
using OneSignalSDK.DotNet;
using OneSignalSDK.DotNet.Core;
using OneSignalSDK.DotNet.Core.Debug;

namespace Pagamentos
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // DOC: https://documentation.onesignal.com/docs/net-sdk-setup

            string oneSignalAppId = Secrets.OneSignalAppId;

            // Enable verbose OneSignal logging to debug issues if needed.
            OneSignal.Debug.LogLevel = LogLevel.VERBOSE;

            // OneSignal Initialization
            OneSignal.Initialize(oneSignalAppId);

            // RequestPermissionAsync will show the notification permission prompt.
            // We recommend removing the following code and instead using an In-App Message to prompt for notification permission (See step 5)
            OneSignal.Notifications.RequestPermissionAsync(true);

            MainPage = new NavigationPage(new MainPage());
        }
    }
}
