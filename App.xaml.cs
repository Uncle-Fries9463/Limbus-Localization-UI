using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Limbus_Localization_UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SplashScreen splash = new SplashScreen("Images/Logo.ico");
            splash.Show(true);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Loaded += (s, args) => splash.Close(fadeoutDuration: new TimeSpan(minutes: 0, seconds: 0, hours: 0));
                mainWindow.Show();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
