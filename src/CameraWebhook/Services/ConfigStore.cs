using System.IO;
using System.Text.Json;
using CameraWebhook.Models;

namespace CameraWebhook.Services;

public sealed class ConfigStore
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
    public string ConfigPath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "CameraWebhook", "config.json");

    public AppConfig Load()
    {
        try
        {
            if (File.Exists(ConfigPath))
                return JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(ConfigPath), Options) ?? new AppConfig();
        }
        catch
        {
            // A malformed file must not prevent the tray agent from starting.
        }
        return new AppConfig();
    }

    public void Save(AppConfig config)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
        var tempPath = ConfigPath + ".tmp";
        File.WriteAllText(tempPath, JsonSerializer.Serialize(config, Options));
        File.Move(tempPath, ConfigPath, true);
    }
}
