using System;
using System.IO;
using System.Text.Json;
using ToggleMute.Models;

namespace ToggleMute.Services;

public interface IConfigService
{
    public AppConfig CurrentConfig { get; set; }

    public AppConfig Load();

    public void Save(AppConfig config);
}

public class ConfigService : IConfigService
{
    private const string ConfigPath = "config.json";

    public AppConfig CurrentConfig { get; set; } = new();

    public AppConfig Load()
    {
        if (File.Exists(ConfigPath) == false)
        {
            CurrentConfig = new AppConfig();
        }
        else
        {
            var json = File.ReadAllText(ConfigPath);
            CurrentConfig = JsonSerializer.Deserialize<AppConfig>(json) ?? throw new ArgumentException();
        }

        return CurrentConfig;
    }

    public void Save(AppConfig config)
    {
        var json = JsonSerializer.Serialize(config);
        File.WriteAllText(ConfigPath, json);
    }
}