using System;
using System.Diagnostics;
using NHotkey.Wpf;
using ToggleMute.Models;

namespace ToggleMute.Services;

public interface IHotkeyService
{
    /// <summary>
    ///     Register or unregister hotkey according to <see cref="HotkeySetting.IsNone" />
    /// </summary>
    public void RegisterOrUnregisterHotkey(HotkeySetting keySetting, Action action);

    public void RegisterHotkey(HotkeySetting keySetting, Action action);

    public void UnregisterHotkey(string name);
}

public class HotkeyService : IHotkeyService
{
    public void RegisterOrUnregisterHotkey(HotkeySetting keySetting, Action action)
    {
        if (keySetting.IsNone() == false)
            RegisterHotkey(keySetting, action);
        else
            UnregisterHotkey(keySetting.Name);
    }

    public void RegisterHotkey(HotkeySetting keySetting, Action action)
    {
        HotkeyManager.Current.AddOrReplace(keySetting.Name, keySetting.Hotkey.Key, keySetting.Hotkey.Modifiers,
            (sender, e) => { action(); });
        Debug.WriteLine($"Hotkey has been registered: {keySetting.Name} with {keySetting}.");
    }

    public void UnregisterHotkey(string name)
    {
        HotkeyManager.Current.Remove(name);
    }
}