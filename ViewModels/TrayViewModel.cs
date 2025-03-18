using CommunityToolkit.Mvvm.Input;
using System.Windows;
using ToggleMute.Views;

namespace ToggleMute.ViewModels
{
    public partial class TrayViewModel
    {
        private SettingsWindow? settingsWindow;

        [RelayCommand]
        private void ShowSettings()
        {
            if (settingsWindow == null)
            {
                settingsWindow = new SettingsWindow();
                settingsWindow.Closed += (sender, e) => { settingsWindow = null; };
                settingsWindow.Show();
            }
            else
            {
                settingsWindow.Activate();
            }
        }

        [RelayCommand]
        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}
