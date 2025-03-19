using System;
using Microsoft.Extensions.Logging;
using NHotkey.Wpf;
using Serilog.Core;
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

public class HotkeyService(ILogger<HotkeyService> logger) : IHotkeyService
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
        logger.LogInformation("Registering hotkey: {Hotkey} for {HotkeyName}", keySetting.Hotkey, keySetting.Name);
        HotkeyManager.Current.AddOrReplace(keySetting.Name, keySetting.Hotkey.Key, keySetting.Hotkey.Modifiers,
            (sender, e) => { action(); });
    }

    public void UnregisterHotkey(string name)
    {
        logger.LogInformation("UnRegistering {HotkeyName}", name);
        HotkeyManager.Current.Remove(name);
    }
}