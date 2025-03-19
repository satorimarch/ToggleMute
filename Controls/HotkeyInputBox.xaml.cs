using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using ToggleMute.Models;
using ToggleMute.ViewModels;

namespace ToggleMute.Controls;

public partial class HotkeyInputBox : UserControl
{
    public HotkeyInputBox()
    {
        InitializeComponent();
    }

    private static readonly Key[] IgnoreKeysArray =
    [
        Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.RightAlt,
        Key.LeftShift, Key.RightShift, Key.LWin, Key.RWin, Key.Return
    ];

    private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        var key = e.Key == Key.System ? e.SystemKey : e.Key;
        if (IgnoreKeysArray.Contains(key)) return;

        if (DataContext is not HotkeySettingViewModel context)
            throw new InvalidOperationException("Data context is null or incorrect type.");

        context.CommitHotkeyCommand.Execute(new Hotkey(key, Keyboard.Modifiers));

        e.Handled = true;
    }
}