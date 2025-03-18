using Microsoft.Extensions.DependencyInjection;
using NHotkey.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToggleMute.Models;

namespace ToggleMute.Services
{
    public interface IAppService
    {
        public void InitFromConfig(AppConfig config);

        public void ResetConfig();

        void UpdateHotkey(HotkeySetting hotkey);

        void RegisterHotkey(HotkeySetting hotkey);

        void UnregisterHotkey(HotkeySetting hotkey);

        void RegisterAllHotkeys(AppConfig config);

        void UnregisterAllHotkeys(AppConfig config);
    }

    public class AppService : IAppService
    {
        private readonly IHotkeyService hotkeyService;
        private readonly IMuteService muteService;
        private readonly IConfigService configService;

        public AppService(IHotkeyService hotkeyService, IMuteService muteService, IConfigService configService)
        {
            this.hotkeyService = hotkeyService;
            this.muteService = muteService;
            this.configService = configService;
        }

        public void InitFromConfig(AppConfig config)
        {
            UnregisterAllHotkeys(config);
            RegisterAllHotkeys(config);
            muteService.IgnoreProcesses = config.IgnoreProcesses;
        }

        public void ResetConfig()
        {
            configService.CurrentConfig = new AppConfig();
            InitFromConfig(configService.CurrentConfig);
        }

        public void UpdateHotkey(HotkeySetting hotkey)
        {
            hotkeyService.RegisterOrUnregisterHotkey(hotkey, GetActionForHotkey(hotkey.Name));
        }

        public void RegisterHotkey(HotkeySetting hotkey)
        {
            hotkeyService.RegisterHotkey(hotkey, GetActionForHotkey(hotkey.Name));
        }

        public void UnregisterHotkey(HotkeySetting hotkey)
        {
            hotkeyService.UnregisterHotkey(hotkey.Name);
        }

        public void RegisterAllHotkeys(AppConfig config)
        {
            Debug.WriteLine("Start to register hotkeys from config.");

            UnregisterAllHotkeys(config);
            foreach (var hotkey in config.Hotkeys)
            {
                hotkeyService.RegisterOrUnregisterHotkey(hotkey, GetActionForHotkey(hotkey.Name));
            }
        }

        public void UnregisterAllHotkeys(AppConfig config)
        {
            foreach (var property in config.Hotkeys)
            {
                hotkeyService.UnregisterHotkey(property.Name);
            }
        }

        /// <summary>
        /// Get the corresponding action from <see cref="MuteService"/> according to the name of the hotkey. 
        /// </summary>
        /// <param name="hotkeyName"></param>
        /// <returns>The Corresponding Action.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private Action GetActionForHotkey(string hotkeyName)
        {
            var method = muteService.GetType().GetMethod(hotkeyName, BindingFlags.Public | BindingFlags.Instance);
            if (method == null || method.ReturnType != typeof(void) || method.GetParameters().Length > 0)
            {
                throw new InvalidOperationException($"Cannot find corresponding method to {hotkeyName}");
            }

            return () => { method.Invoke(muteService, null); };
        }
    }
}
