using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using NHotkey;
using System.Linq;
using System.Windows.Forms;
using ToggleMute.Models;
using ToggleMute.Services;

namespace ToggleMute.ViewModels
{
    public partial class HotkeySettingViewModel : ObservableObject
    {
        [ObservableProperty]
        private HotkeySetting _hotkey;

        private readonly IConfigService _configService;
        private readonly IAppService _appService;

        public HotkeySettingViewModel(HotkeySetting hotkey)
        {
            Hotkey = hotkey;

            _configService = App.Current.ServiceProvider.GetRequiredService<IConfigService>();
            _appService = App.Current.ServiceProvider.GetRequiredService<IAppService>();
        }

        [RelayCommand]
        private void CommitHotkey()
        {
            try
            {
                _appService.UpdateHotkey(Hotkey);
            }
            catch (HotkeyAlreadyRegisteredException)
            {
                MessageBox.Show("Hotkey has been registered, please change it and try again.", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            var existingHotkey = _configService.CurrentConfig.Hotkeys.FirstOrDefault(h => h.Name == Hotkey.Name);
            if (existingHotkey != null)
            {
                existingHotkey = Hotkey;
            }

            _configService.Save(_configService.CurrentConfig);
        }
    }
}