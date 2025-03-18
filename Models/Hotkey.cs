using System.Text;
using System.Windows.Input;

namespace ToggleMute.Models;

public class Hotkey(Key key = Key.None, ModifierKeys modifiers = ModifierKeys.None)
{
    public Key Key { get; } = key;

    public ModifierKeys Modifiers { get; } = modifiers;

    public bool IsNone()
    {
        return Key == Key.None && Modifiers == ModifierKeys.None;
    }

    public override string ToString()
    {
        var text = new StringBuilder();

        if (Modifiers.HasFlag(ModifierKeys.Control))
            text.Append("Ctrl + ");
        if (Modifiers.HasFlag(ModifierKeys.Alt))
            text.Append("Alt + ");
        if (Modifiers.HasFlag(ModifierKeys.Shift))
            text.Append("Shift + ");
        if (Modifiers.HasFlag(ModifierKeys.Windows))
            text.Append("Win + ");

        text.Append(Key.ToString());

        return text.ToString();
    }
}