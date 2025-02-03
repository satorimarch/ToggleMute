using System.IO;
using System.Text.Json;
using ToggleMute.Models;

namespace ToggleMute.Services
{
    public interface IAppConfigService
    {
        public AppConfig CurrentConfig { get; set; }

        public AppConfig Load();

        public void Save(AppConfig config);
    }

    public class AppConfigService : IAppConfigService
    {
        private static readonly string ConfigPath = "config.json";

        public AppConfig CurrentConfig { get; set; } = new();

        /// <remarks>
        /// TODO: Notify the outside on error occurs, instead of create new config silently.
        /// </remarks>
        public AppConfig Load()
        {
            if (File.Exists(ConfigPath) == false)
            {
                CurrentConfig = new AppConfig();
                return CurrentConfig;
            }

            var json = File.ReadAllText(ConfigPath);
            CurrentConfig = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
            return CurrentConfig;
        }

        public void Save(AppConfig config)
        {
            var json = JsonSerializer.Serialize(config);
            File.WriteAllText(ConfigPath, json);
        }
    }
}
