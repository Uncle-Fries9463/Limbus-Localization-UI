using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.IO;
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
            if (File.Exists(@"[Ресурсы]\Sprites\Спрайты.zip"))
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(@"[Ресурсы]\Sprites\Спрайты.zip", @"[Ресурсы]\Sprites");
                File.Delete(@"[Ресурсы]\Sprites\Спрайты.zip");
            }

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

        public App() : base()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Откуда эта ошибка вообще\n" + "Отправьте мне Ошибка.log из папки с программой", "Чё", MessageBoxButton.OK, MessageBoxImage.Error);
            File.WriteAllText("Ошибка.log", $"{e.Exception.Message}\nОшибка: {e.Exception.StackTrace}\n\n{e.Exception.InnerException}\n\n{e.Exception.Data}\n\n{e.Exception.HelpLink}\n\n{e.Exception.HResult}\n\n{e.Exception.TargetSite}");
            Environment.Exit(0);
        }
    }
}
