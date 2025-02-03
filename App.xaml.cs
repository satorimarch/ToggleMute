using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;
using NHotkey;
using System;
using System.Windows;
using ToggleMute.Models;
using ToggleMute.Services;
using ToggleMute.ViewModels;

namespace ToggleMute
{
    public partial class App : Application
    {
        private TaskbarIcon? _trayIcon;
        private readonly IAppConfigService _configService;
        private readonly IMuteService _muteService;

        public IServiceProvider ServiceProvider { get; }

        public static new App Current => (App)Application.Current;

        public App()
        {
            ServiceProvider = ConfigureServices();
            _configService = ServiceProvider.GetRequiredService<IAppConfigService>();
            _muteService = ServiceProvider.GetRequiredService<IMuteService>();
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IAppConfigService, AppConfigService>();
            services.AddSingleton<IMuteService, MuteService>();

            services.AddSingleton<TrayViewModel>();
            services.AddSingleton<SettingsViewModel>();

            return services.BuildServiceProvider();
        }

        void LoadConfig(AppConfig config)
        {
            HotkeyService.RegisterAllFromConfig(config);
            _muteService.IgnoreProcesses = config.IgnoreProcesses;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                LoadConfig(_configService.Load());
            }
            catch (HotkeyAlreadyRegisteredException)
            {
                var result = MessageBox.Show($"The settings are incorrect, do you want to reset?", "Error",
                    MessageBoxButton.YesNo, MessageBoxImage.Error);

                if (result == MessageBoxResult.Yes)
                {
                    _configService.Save(new AppConfig());
                    LoadConfig(_configService.CurrentConfig);
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }

            _trayIcon = (TaskbarIcon)FindResource("TrayIcon");
            _trayIcon.DataContext = ServiceProvider.GetRequiredService<TrayViewModel>();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            HotkeyService.UnregisterAllFromConfig(_configService.CurrentConfig);
            _trayIcon?.Dispose();
        }
    }
}