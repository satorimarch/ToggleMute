using CommunityToolkit.Mvvm.Input;
using System.Windows;
using ToggleMute.Services;
using ToggleMute.Views;

namespace ToggleMute.ViewModels
{
    public partial class TrayViewModel
    {
        private readonly IAppConfigService _configService;
        private SettingsWindow? _settingsWindow;

        public TrayViewModel(IAppConfigService service)
        {
            _configService = service;
        }

        [RelayCommand]
        private void ShowSettings()
        {
            if (_settingsWindow == null)
            {
                _settingsWindow = new SettingsWindow(_configService);
                _settingsWindow.Closed += (sender, e) => { _settingsWindow = null; };
                _settingsWindow.Show();
            }
            else
            {
                _settingsWindow.Activate();
            }
        }

        [RelayCommand]
        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}
