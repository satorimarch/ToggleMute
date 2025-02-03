using System.IO;
using System.Text.Json;
using ToggleMute.Models;

namespace ToggleMute.Services
{
    public interface IAppConfigService
    {
        public AppConfig Config { get; set; }

        public AppConfig Load();

        public void Save(AppConfig config);
    }

    public class AppConfigService : IAppConfigService
    {
        private static readonly string ConfigPath = "config.json";

        public AppConfig Config { get; set; } = new();

        /// <remarks>
        /// TODO: Notify the outside on error occurs, instead of create new config silently.
        /// </remarks>
        public AppConfig Load()
        {
            if (File.Exists(ConfigPath) == false)
            {
                Config = new AppConfig();
                return Config;
            }

            var json = File.ReadAllText(ConfigPath);
            Config = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
            return Config;
        }

        public void Save(AppConfig config)
        {
            var json = JsonSerializer.Serialize(config);
            File.WriteAllText(ConfigPath, json);
        }
    }
}
