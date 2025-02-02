using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using ToggleMute.Models;
using ToggleMute.Services;

namespace ToggleMute.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IAppConfigService _configService;

        [ObservableProperty]
        private List<HotkeySettingViewModel> _hotkeys;

        public SettingsViewModel(IAppConfigService configService)
        {
            _configService = configService;

            Hotkeys = new(_configService.Config.Hotkeys.Select((hotkey) => new HotkeySettingViewModel(hotkey)));
        }

        [RelayCommand]
        private void ResetConfig()
        {
            if (MessageBox.Show("Are you sure you want to reset all settings?", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            HotkeyService.UnregisterAllFromConfig(_configService.Config);

            _configService.Save(new AppConfig());

            Hotkeys = new(_configService.Config.Hotkeys.Select((hotkey) => new HotkeySettingViewModel(hotkey)));

            MessageBox.Show("Settings have been reset.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}