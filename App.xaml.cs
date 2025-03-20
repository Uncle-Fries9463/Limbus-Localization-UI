using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.IO;
using System.Windows.Interop;
using System.Windows.Media;
using NLog;

namespace Limbus_Localization_UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        protected override void OnStartup(StartupEventArgs e)
        {
            if (File.Exists(@"[Ресурсы]\Sprites\Sprites.zip"))
            {
                try
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(@"[Ресурсы]\Sprites\Sprites.zip", @"[Ресурсы]\Sprites");
                }
                catch { }
                File.Delete(@"[Ресурсы]\Sprites\Sprites.zip");
            }

            base.OnStartup(e);

            SetupExceptionHandling();

            SplashScreen splash = new SplashScreen("Images/Logo.ico");
            splash.Show(true);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Loaded += (s, args) => splash.Close(fadeoutDuration: new TimeSpan(minutes: 0, seconds: 0, hours: 0));
                mainWindow.Show();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }


        // Я ХОЧУ ВЕРИТЬ, ЧТО ОНО РАБОТАЕТ
        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }
        private void LogUnhandledException(Exception exception, string source)
        {
            string message = $"Unhandled exception ({source})";
            try
            {
                System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
                
                try
                {
                    File.AppendAllText(@"[Ресурсы]\& Stringtypes\Error Log.txt", $"[{DateTime.Now.ToString("HH:mm:ss")}] Exception:\n{exception.ToString()}\n---------------------------------------------------------------------------------\n\n\n");
                }
                catch { }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception in LogUnhandledException");
            }
            finally
            {
                _logger.Error(exception, message);
            }
        }
    }
}
