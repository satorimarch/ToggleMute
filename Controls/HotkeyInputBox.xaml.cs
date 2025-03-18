using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ToggleMute.Models;
using ToggleMute.ViewModels;

namespace ToggleMute.Controls
{
    public partial class HotkeyInputBox : UserControl
    {
        public HotkeyInputBox()
        {
            InitializeComponent();
        }

        private static readonly Key[] IgnoreKeysArray =
        [
            Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.RightAlt, Key.System,
            Key.LeftShift, Key.RightShift, Key.LWin, Key.RWin, Key.Return
        ];

        private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IgnoreKeysArray.Contains(e.Key))
            {
                return;
            }

            if (DataContext is not HotkeySettingViewModel context)
            {
                throw new InvalidOperationException("Datacontext is null or incorrect type.");
            }

            Key key;
            ModifierKeys modifiers;

            if (e.Key == Key.Escape)
            {
                key = Key.None;
                modifiers = ModifierKeys.None;
            }
            else
            {
                key = e.Key;
                modifiers = Keyboard.Modifiers;
            }

            context.CommitHotkeyCommand.Execute(new Hotkey(key, modifiers));

            e.Handled = true;
        }
    }
}