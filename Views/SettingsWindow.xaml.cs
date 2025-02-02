using System.Windows;
using ToggleMute.Services;
using ToggleMute.ViewModels;

namespace ToggleMute.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(IAppConfigService configService)
        {
            InitializeComponent();
            DataContext = new SettingsViewModel(configService);
        }
    }
}