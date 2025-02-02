using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ToggleMute.Models;
using ToggleMute.ViewModels;

namespace ToggleMute.Controls
{
    public partial class HotkeyInputBox : UserControl
    {
        public static readonly DependencyProperty HotkeyProperty =
                DependencyProperty.Register(
                    "Hotkey",
                    typeof(HotkeySetting),
                    typeof(HotkeyInputBox),
                    new FrameworkPropertyMetadata(
                        null,
                        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
                    )
                );

        public HotkeySetting Hotkey
        {
            get => (HotkeySetting)GetValue(HotkeyProperty);
            set => SetValue(HotkeyProperty, value);
        }

        public HotkeyInputBox()
        {
            InitializeComponent();
        }

        private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl ||
                e.Key == Key.LeftAlt || e.Key == Key.RightAlt || e.Key == Key.System ||
                e.Key == Key.LeftShift || e.Key == Key.RightShift ||
                e.Key == Key.LWin || e.Key == Key.RWin || e.Key == Key.Return)
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

            Hotkey = new HotkeySetting(Hotkey.Name, key, modifiers);

            context.CommitHotkeyCommand.Execute(null);

            e.Handled = true;
        }
    }
}