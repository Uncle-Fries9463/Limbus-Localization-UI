﻿using NLog;
using System.Windows;
using static LC_Localization_Task_Absolute.Requirements;


namespace LC_Localization_Task_Absolute;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        SetupExceptionHandling();

        SplashScreen StartupSplash = new SplashScreen("UI/Logo.png");
        StartupSplash.Show(true, topMost: true);

        Dispatcher.BeginInvoke(new Action(() =>
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Loaded += (s, args) => StartupSplash.Close(fadeoutDuration: new TimeSpan(minutes: 0, seconds: 0, hours: 0));
            mainWindow.Show();
        }), System.Windows.Threading.DispatcherPriority.Background);
    }

    private static Logger _logger = LogManager.GetCurrentClassLogger();

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
                rin(exception.ToString());

                //File.AppendAllText(@"[Ресурсы]\& Stringtypes\Error Log.txt", $"[{DateTime.Now.ToString("HH:mm:ss")}] Ещё один на доске позора для злодеев:\n{exception.ToString()}\n---------------------------------------------------------------------------------\n\n\n");
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

