using Microsoft.Win32;

namespace CameraWebhook.Services;

public interface ICameraUsageDetector
{
    bool IsCameraInUse();
}

public sealed class CameraUsageDetector : ICameraUsageDetector
{
    private const string WebcamKey = @"Software\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam";

    public bool IsCameraInUse()
    {
        try
        {
            using var root = Registry.CurrentUser.OpenSubKey(WebcamKey);
            return root is not null && HasActiveEntry(root);
        }
        catch
        {
            return false;
        }
    }

    internal static bool HasActiveEntry(RegistryKey key)
    {
        var start = ReadInt64(key.GetValue("LastUsedTimeStart"));
        var stop = ReadInt64(key.GetValue("LastUsedTimeStop"));
        if (start > 0 && stop == 0)
            return true;

        foreach (var name in key.GetSubKeyNames())
        {
            try
            {
                using var child = key.OpenSubKey(name);
                if (child is not null && HasActiveEntry(child))
                    return true;
            }
            catch
            {
                // An inaccessible application entry does not invalidate other entries.
            }
        }
        return false;
    }

    private static long ReadInt64(object? value) => value switch
    {
        long number => number,
        int number => number,
        _ => 0
    };
}

