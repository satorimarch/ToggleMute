using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using ToggleMute.ViewModels;

namespace ToggleMute.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = App.Current.ServiceProvider.GetRequiredService<SettingsViewModel>();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var context = DataContext as SettingsViewModel;
            context?.CommitIgnoreProcessesCommand.Execute(null);
        }
    }
}