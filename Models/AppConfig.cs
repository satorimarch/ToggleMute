using System;
using System.Collections.Generic;
using ToggleMute.Services;

namespace ToggleMute.Models
{
    public class AppConfig
    {
        /// <summary>
        /// No hotkeys are set by default.
        /// </summary>
        /// <remarks>
        /// <b>Caution</b>: The hotkey names must exactly match the method names in <see cref="IMuteService"/>.
        /// </remarks>
        public List<HotkeySetting> Hotkeys { get; set; } = [
            new HotkeySetting("ToggleMuteActiveWindow"),
            new HotkeySetting("MuteActiveWindow"),
            new HotkeySetting("UnmuteActiveWindow"),
            new HotkeySetting("MuteOtherWindows"),
            new HotkeySetting("UnmuteOtherWindows")
        ];

        public HashSet<string> IgnoreProcesses { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }
}