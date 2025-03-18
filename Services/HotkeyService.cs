using System;
using System.Diagnostics;
using NHotkey.Wpf;
using ToggleMute.Models;

namespace ToggleMute.Services;

public interface IHotkeyService
{
    /// <summary>
    ///     Register or unregister hotkey according to <see cref="HotkeySetting.HasHotkey" />
    /// </summary>
    public void RegisterOrUnregisterHotkey(HotkeySetting hotkey, Action action);

    public void RegisterHotkey(HotkeySetting hotkey, Action action);

    public void UnregisterHotkey(string name);
}

public class HotkeyService : IHotkeyService
{
    public void RegisterOrUnregisterHotkey(HotkeySetting hotkey, Action action)
    {
        if (hotkey.HasHotkey())
            RegisterHotkey(hotkey, action);
        else
            UnregisterHotkey(hotkey.Name);
    }

    public void RegisterHotkey(HotkeySetting hotkey, Action action)
    {
        HotkeyManager.Current.AddOrReplace(hotkey.Name, hotkey.Key, hotkey.Modifiers, (sender, e) => { action(); });
        Debug.WriteLine($"Hotkey has been registered: {hotkey.Name} with {hotkey}.");
    }

    public void UnregisterHotkey(string name)
    {
        HotkeyManager.Current.Remove(name);
    }
}