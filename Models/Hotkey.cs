using System.Windows.Input;

namespace ToggleMute.Models;

public class Hotkey(Key key, ModifierKeys modifiers)
{
    public Key Key { get; } = key;

    public ModifierKeys Modifiers { get; } = modifiers;
}