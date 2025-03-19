using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using ToggleMute.Models;

namespace ToggleMute.Services;

public interface IAppService
{
    public void InitFromConfig(AppConfig config);

    void UpdateHotkey(HotkeySetting keySetting);

    void UnregisterAllHotkeys(AppConfig config);
}

public class AppService(IHotkeyService hotkeyService, IMuteService muteService, ILogger<AppService> logger)
    : IAppService
{
    public void InitFromConfig(AppConfig config)
    {
        logger.LogInformation("Initializing config");
        RegisterAllHotkeys(config);
        muteService.IgnoreProcesses = config.IgnoreProcesses;
    }

    public void UpdateHotkey(HotkeySetting keySetting)
    {
        hotkeyService.RegisterOrUnregisterHotkey(keySetting, GetActionForHotkey(keySetting.Name));
    }

    public void RegisterAllHotkeys(AppConfig config)
    {
        UnregisterAllHotkeys(config);
        foreach (var hotkey in config.Hotkeys)
            hotkeyService.RegisterOrUnregisterHotkey(hotkey, GetActionForHotkey(hotkey.Name));
    }

    public void UnregisterAllHotkeys(AppConfig config)
    {
        foreach (var property in config.Hotkeys) hotkeyService.UnregisterHotkey(property.Name);
    }

    /// <summary>
    ///     Get the corresponding action from <see cref="MuteService" /> according to the name of the hotkey.
    /// </summary>
    /// <param name="hotkeyName"></param>
    /// <returns>The Corresponding Action.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private Action GetActionForHotkey(string hotkeyName)
    {
        var method = muteService.GetType().GetMethod(hotkeyName, BindingFlags.Public | BindingFlags.Instance);
        if (method == null || method.ReturnType != typeof(void) || method.GetParameters().Length > 0)
            throw new InvalidOperationException($"Cannot find corresponding method to {hotkeyName}");

        return () => { method.Invoke(muteService, null); };
    }
}