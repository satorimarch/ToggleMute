using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NHotkey;
using System.Windows.Forms;
using ToggleMute.Models;
using ToggleMute.Services;

namespace ToggleMute.ViewModels
{
    public partial class HotkeySettingViewModel : ObservableObject
    {
        [ObservableProperty]
        private HotkeySetting _hotkey;

        public HotkeySettingViewModel(HotkeySetting hotkey)
        {
            Hotkey = hotkey;
        }

        [RelayCommand]
        private void CommitHotkey()
        {
            if (Hotkey.HasHotkey())
            {
                try
                {
                    HotkeyService.RegisterHotkey(Hotkey);
                }
                catch (HotkeyAlreadyRegisteredException)
                {
                    MessageBox.Show("Hotkey has been registered, please change it and try again.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                HotkeyService.UnregisterHotkey(Hotkey.Name);
            }
        }
    }
}