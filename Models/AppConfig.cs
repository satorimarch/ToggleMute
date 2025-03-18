using System;
using System.Collections.Generic;
using ToggleMute.Services;

namespace ToggleMute.Models;

public class AppConfig
{
    /// <summary>
    ///     No hotkeys are set by default.
    /// </summary>
    /// <remarks>
    ///     <b>Caution</b>: The hotkey names must exactly match the method names in <see cref="IMuteService" />.
    /// </remarks>
    public List<HotkeySetting> Hotkeys { get; set; } =
    [
        new("ToggleMuteActiveWindow"),
        new("MuteActiveWindow"),
        new("UnmuteActiveWindow"),
        new("MuteOtherWindows"),
        new("UnmuteOtherWindows")
    ];

    public HashSet<string> IgnoreProcesses { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}