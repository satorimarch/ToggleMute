using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToggleMute.Models;
using ToggleMute.Services;

namespace ToggleMute.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IConfigService _configService;
    private readonly IAppService _appService;
    private readonly ILanguageService _langService;

    [ObservableProperty]
    private List<HotkeySettingViewModel> _hotkeys = [];

    [ObservableProperty]
    private string _ignoredProcessesText = string.Empty;

    public SettingsViewModel(IConfigService configService, IAppService appService, ILanguageService langService)
    {
        _configService = configService;
        _appService = appService;
        _langService = langService;

        LoadFromConfig(configService.CurrentConfig);
    }

    partial void OnIgnoredProcessesTextChanged(string value)
    {
        _configService.CurrentConfig.IgnoreProcesses =
            new HashSet<string>(IgnoredProcessesText.Split(',').Select(s => s.Trim()));
        SaveConfig();
    }

    private void LoadFromConfig(AppConfig config)
    {
        Hotkeys = new List<HotkeySettingViewModel>(config.Hotkeys.Select(hotkey => new HotkeySettingViewModel(hotkey)));
        IgnoredProcessesText = string.Join(',', config.IgnoreProcesses);
    }

    private AppConfig ToConfig()
    {
        return new AppConfig
        {
            Hotkeys = new List<HotkeySetting>(Hotkeys.Select(hotkeyViewModel => hotkeyViewModel.Hotkey)),
            IgnoreProcesses = new HashSet<string>(IgnoredProcessesText.Split(',').Select(s => s.Trim()))
        };
    }

    private void SaveConfig()
    {
        _configService.Save(ToConfig());
        _appService.InitFromConfig(_configService.CurrentConfig);
    }

    [RelayCommand]
    private void ResetConfig()
    {
        if (MessageBox.Show(_langService.GetText("ResetMessageBox"), _langService.GetText("Warning"),
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            return;

        _configService.Save(new AppConfig());
        _appService.InitFromConfig(_configService.CurrentConfig);
        LoadFromConfig(_configService.CurrentConfig);

        MessageBox.Show(_langService.GetText("ResetMessageBoxOk"), _langService.GetText("Info"), MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    [RelayCommand]
    private void ChangeLanguage(string lang)
    {
        _langService.ChangeLanguage(lang);
    }
}