using System.Linq;
using System.Windows.Forms;
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

    public HotkeySettingViewModel(HotkeySetting hotkey)
    {
        Hotkey = hotkey;

        configService = App.Current.ServiceProvider.GetRequiredService<IConfigService>();
        appService = App.Current.ServiceProvider.GetRequiredService<IAppService>();
    }

    [RelayCommand]
    private void CommitHotkey(Hotkey hotkey)
    {
        Hotkey = new HotkeySetting(Hotkey.Name, hotkey.Key, hotkey.Modifiers);

        try
        {
            appService.UpdateHotkey(Hotkey);
        }
        catch (HotkeyAlreadyRegisteredException)
        {
            MessageBox.Show("Hotkey has been registered, please change it and try again.", "Warning",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        var index = configService.CurrentConfig.Hotkeys.FindIndex(h => h.Name == Hotkey.Name);
        configService.CurrentConfig.Hotkeys[index] = Hotkey;

        configService.Save(configService.CurrentConfig);
    }
}
