using System;
using System.Collections.Generic;

namespace ToggleMute.Models
{
    /// <summary>
    /// No hotkeys are set on default.
    /// </summary>
    public class AppConfig
    {
        /// <remarks>
        /// Caution: The name of hotkey must be same with static method name in <see cref="MuteService"/>.
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