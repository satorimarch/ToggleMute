using Microsoft.Extensions.DependencyInjection;
using NHotkey.Wpf;
using System;
using System.Diagnostics;
using System.Reflection;
using ToggleMute.Models;

namespace ToggleMute.Services
{
    public static class HotkeyService
    {
        /// <summary>
        /// Register or unregister hotkey according to <see cref="HotkeySetting.HasHotkey"/>
        /// </summary>
        public static void RegisterOrUnregisterHotkey(HotkeySetting hotkey)
        {
            if (hotkey.HasHotkey())
            {
                RegisterHotkey(hotkey);
            }
            else
            {
                UnregisterHotkey(hotkey.Name);
            }
        }

        public static void RegisterHotkey(HotkeySetting hotkey)
        {

            var action = GetActionForHotkey(hotkey.Name);
            HotkeyManager.Current.AddOrReplace(hotkey.Name, hotkey.Key, hotkey.Modifiers, (sender, e) => { action(); });
            Debug.WriteLine($"Hotkey has been registered: {hotkey.Name} with {hotkey}.");
        }

        public static void UnregisterHotkey(string name)
        {
            HotkeyManager.Current.Remove(name);
        }

        public static void RegisterAllFromConfig(AppConfig config)
        {
            Debug.WriteLine("Start to register hotkeys from config.");
            UnregisterAllFromConfig(config);
            foreach (var hotkey in config.Hotkeys)
            {
                RegisterOrUnregisterHotkey(hotkey);
            }
        }

        public static void UnregisterAllFromConfig(AppConfig config)
        {
            foreach (var property in config.Hotkeys)
            {
                HotkeyManager.Current.Remove(property.Name);
            }
        }

        /// <summary>
        /// Get the corresponding action from <see cref="MuteService"/> according to the name of the hotkey. 
        /// </summary>
        /// <param name="hotkeyName"></param>
        /// <returns>The Corresponding Action.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static Action GetActionForHotkey(string hotkeyName)
        {
            var muteService = App.Current.ServiceProvider.GetRequiredService<IMuteService>();
            var method = muteService.GetType().GetMethod(hotkeyName, BindingFlags.Public | BindingFlags.Instance);
            if (method == null || method.ReturnType != typeof(void) || method.GetParameters().Length > 0)
            {
                throw new InvalidOperationException($"Cannot find coressponding method to {hotkeyName}");
            }

            return () => { method.Invoke(muteService, null); };
        }
    }
}
