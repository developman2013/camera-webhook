namespace CameraWebhook.Models;

public sealed class AppConfig
{
    public bool StartWithWindows { get; set; }
    public int PollIntervalMilliseconds { get; set; } = 1000;
    public EndpointConfig CameraOn { get; set; } = new()
    {
        Body = "{\n  \"event\": \"{{event}}\",\n  \"timestamp\": \"{{timestamp}}\",\n  \"machine\": \"{{machine}}\"\n}"
    };
    public EndpointConfig CameraOff { get; set; } = new()
    {
        Body = "{\n  \"event\": \"{{event}}\",\n  \"timestamp\": \"{{timestamp}}\",\n  \"machine\": \"{{machine}}\"\n}"
    };
}

public sealed class EndpointConfig
{
    public bool Enabled { get; set; }
    public string Url { get; set; } = "";
    public string Method { get; set; } = "POST";
    public string HeadersJson { get; set; } = "{}";
    public string Body { get; set; } = "";
}

