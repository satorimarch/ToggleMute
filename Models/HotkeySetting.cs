using System.Text;
using System.Text.Json.Serialization;
using System.Windows.Input;
using ToggleMute.Services;

namespace ToggleMute.Models;

/// <remarks>
///     Represent unregister hotkey when <see cref="IsNone"/>.
/// </remarks>
public class HotkeySetting
{
    public HotkeySetting(string name) : this(name, new Hotkey())
    {
    }

    [JsonConstructorAttribute]
    public HotkeySetting(string name, Hotkey hotkey)
    {
        Name = name;
        Hotkey = hotkey;
    }

    /// <summary>
    ///     Used as identifier for hotkey service. Must be identical to <see cref="MuteService" /> method.
    /// </summary>
    public string Name { get; }

    public Hotkey Hotkey { get; }

    public override string ToString()
    {
        return Hotkey.ToString();
    }

    public bool IsNone()
    {
        return Hotkey.IsNone();
    }
}