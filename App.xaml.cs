using System;
using System.Windows;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;
using NHotkey;
using ToggleMute.Models;
using ToggleMute.Services;
using ToggleMute.ViewModels;

namespace ToggleMute;

public partial class App : Application
{
    private TaskbarIcon? trayIcon;
    private readonly IConfigService configService;
    private readonly IAppService appService;

    public IServiceProvider ServiceProvider { get; }

    public new static App Current => (App)Application.Current;

    public App()
    {
        ServiceProvider = ConfigureServices();
        configService = ServiceProvider.GetRequiredService<IConfigService>();
        appService = ServiceProvider.GetRequiredService<IAppService>();

        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        DispatcherUnhandledException += App_DispatcherUnhandledException;
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IConfigService, ConfigService>();
        services.AddSingleton<IMuteService, MuteService>();
        services.AddSingleton<IHotkeyService, HotkeyService>();
        services.AddSingleton<IAppService, AppService>();

        services.AddSingleton<SettingsViewModel>();

        return services.BuildServiceProvider();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        trayIcon = FindResource("TrayIcon") as TaskbarIcon ??
                   throw new InvalidOperationException($"{nameof(trayIcon)} can't be null.");

        try
        {
            appService.InitFromConfig(configService.Load());
        }
        catch (HotkeyAlreadyRegisteredException ex)
        {
            var result = MessageBox.Show($"Hotkey {ex.Name} has been registered, do you want to open the setting window?",
                "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var context = trayIcon.DataContext as TrayViewModel ?? throw new InvalidOperationException();
                context.ShowSettingsCommand.Execute(null);
            }
        }
        catch
        {
            var result = MessageBox.Show("The settings are incorrect, do you want to reset?", "Error",
                MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                configService.Save(new AppConfig());
                appService.InitFromConfig(configService.CurrentConfig);
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        appService.UnregisterAllHotkeys(configService.CurrentConfig);
        trayIcon?.Dispose();
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = (Exception)e.ExceptionObject;
        MessageBox.Show($"Unhandled exception: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show($"Dispatcher unhandled exception: {e.Exception.Message}", "Error", MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
    }
}