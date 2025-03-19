using System.Windows.Forms;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NHotkey;
using ToggleMute.Models;
using ToggleMute.Services;

namespace ToggleMute.ViewModels;

public partial class HotkeySettingViewModel : ObservableObject
{
    [ObservableProperty]
    private HotkeySetting _hotkey;

    private readonly IConfigService configService;
    private readonly IAppService appService;
    private readonly ILanguageService langService;

    public string HotkeyText { get; }

    public HotkeySettingViewModel(HotkeySetting hotkey)
    {
        Hotkey = hotkey;

        configService = App.Current.ServiceProvider.GetRequiredService<IConfigService>();
        appService = App.Current.ServiceProvider.GetRequiredService<IAppService>();
        langService = App.Current.ServiceProvider.GetRequiredService<ILanguageService>();

        HotkeyText = langService.GetText(Hotkey.Name);
    }

    [RelayCommand]
    private void CommitHotkey(Hotkey hotkey)
    {
        if (hotkey.Modifiers == ModifierKeys.None && hotkey.Key != Key.Escape) return;
        if (hotkey.Key == Key.Escape) hotkey = new Hotkey();

        Hotkey = new HotkeySetting(Hotkey.Name, hotkey);

        try
        {
            appService.UpdateHotkey(Hotkey);
        }
        catch (HotkeyAlreadyRegisteredException)
        {
            MessageBox.Show(langService.GetText("RetryHotkeyMessageBox"), langService.GetText("Warning"),
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        var index = configService.CurrentConfig.Hotkeys.FindIndex(h => h.Name == Hotkey.Name);
        configService.CurrentConfig.Hotkeys[index] = Hotkey;

        configService.Save(configService.CurrentConfig);
    }
}