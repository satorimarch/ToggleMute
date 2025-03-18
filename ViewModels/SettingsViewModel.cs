using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using ToggleMute.Models;
using ToggleMute.Services;

namespace ToggleMute.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IConfigService _configService;
        private readonly IAppService _appService;

        [ObservableProperty]
        private List<HotkeySettingViewModel> _hotkeys = [];

        [ObservableProperty]
        private string _ignoredProcessesText = string.Empty;

        public SettingsViewModel(IConfigService configService, IHotkeyService hotkeyService, IMuteService muteService, IAppService appService)
        {
            _configService = configService;
            _appService = appService;

            LoadFromConfig(configService.CurrentConfig);
        }

        partial void OnIgnoredProcessesTextChanged(string value)
        {
            _configService.CurrentConfig.IgnoreProcesses = new HashSet<string>(IgnoredProcessesText.Split(',').Select(s => s.Trim()));
            SaveConfig();
        }

        private void LoadFromConfig(AppConfig config)
        {
            Hotkeys = new(config.Hotkeys.Select((hotkey) => new HotkeySettingViewModel(hotkey)));
            IgnoredProcessesText = string.Join(',', config.IgnoreProcesses);
        }

        private AppConfig ToConfig()
        {
            return new()
            {
                Hotkeys = new(Hotkeys.Select(hotkeyViewModel => hotkeyViewModel.Hotkey)),
                IgnoreProcesses = new HashSet<string>(IgnoredProcessesText.Split(',').Select(s => s.Trim()))
            };
        }

        private void SaveConfig()
        {
            _configService.Save(this.ToConfig());
            _appService.InitFromConfig(_configService.CurrentConfig);
        }

        [RelayCommand]
        private void ResetConfig()
        {
            if (MessageBox.Show("Are you sure you want to reset all settings?", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            this.LoadFromConfig(new AppConfig());

            MessageBox.Show("Settings have been reset.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}