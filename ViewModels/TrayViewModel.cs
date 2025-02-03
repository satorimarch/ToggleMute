using CommunityToolkit.Mvvm.Input;
using System.Windows;
using ToggleMute.Services;
using ToggleMute.Views;

namespace ToggleMute.ViewModels
{
    public partial class TrayViewModel
    {
        private SettingsWindow? _settingsWindow;

        [RelayCommand]
        private void ShowSettings()
        {
            if (_settingsWindow == null)
            {
                _settingsWindow = new SettingsWindow();
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
