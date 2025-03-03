﻿using CommunityToolkit.Mvvm.ComponentModel;
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
        private readonly IAppConfigService _configService;

        [ObservableProperty]
        private List<HotkeySettingViewModel> _hotkeys;

        [ObservableProperty]
        private string _ignoredProcessesText;

        private readonly IMuteService _muteService;

        public SettingsViewModel(IAppConfigService configService, IMuteService muteService)
        {
            _configService = configService;

            _muteService = muteService;

            Hotkeys = new(_configService.CurrentConfig.Hotkeys.Select((hotkey) => new HotkeySettingViewModel(hotkey)));

            _ignoredProcessesText = string.Join(',', configService.CurrentConfig.IgnoreProcesses);
        }

        [RelayCommand]
        private void CommitIgnoreProcesses()
        {
            Debug.WriteLine("Commiting ignore processes.");
            _configService.CurrentConfig.IgnoreProcesses.Clear();
            _configService.CurrentConfig.IgnoreProcesses.UnionWith(IgnoredProcessesText.Split(',').Select(s => s.Trim()));
            _muteService.IgnoreProcesses = _configService.CurrentConfig.IgnoreProcesses;
        }

        [RelayCommand]
        private void ResetConfig()
        {
            if (MessageBox.Show("Are you sure you want to reset all settings?", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            HotkeyService.UnregisterAllFromConfig(_configService.CurrentConfig);

            _configService.Save(new AppConfig());

            Hotkeys = new(_configService.CurrentConfig.Hotkeys.Select((hotkey) => new HotkeySettingViewModel(hotkey)));

            MessageBox.Show("Settings have been reset.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void SaveConfig()
        {
            _configService.CurrentConfig.Hotkeys = new(Hotkeys.Select(hotkeyViewModel => hotkeyViewModel.Hotkey));
            _configService.Save(_configService.CurrentConfig);

            MessageBox.Show("Settings have been saved.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}