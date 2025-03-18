using System.Text;
using System.Windows.Input;
using ToggleMute.Services;

namespace ToggleMute.Models;

/// <remarks>
///     Represent no to set hotkey when <see cref="Key" /> is None and <see cref="Modifiers" /> is None,
///     which can be checked by <see cref="HasHotkey" />.
/// </remarks>
public class HotkeySetting(string name, Key key = Key.None, ModifierKeys modifiers = ModifierKeys.None)
{
    /// <summary>
    ///     Used as identifier for hotkey service. Must be identical to <see cref="MuteService" /> method.
    /// </summary>
    public string Name { get; } = name;

    public Key Key { get; } = key;

    public ModifierKeys Modifiers { get; } = modifiers;

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

    public bool HasHotkey()
    {
        return Key != Key.None && Modifiers != ModifierKeys.None;
    }
}