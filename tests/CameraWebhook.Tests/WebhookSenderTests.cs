using CameraWebhook.Services;
using Xunit;

namespace CameraWebhook.Tests;

public sealed class WebhookSenderTests
{
    [Fact]
    public void Expand_ReplacesEventAndMachineVariables()
    {
        var result = WebhookSender.Expand("{{event}}/{{machine}}", true);

        Assert.Equal($"camera-on/{Environment.MachineName}", result);
    }

    [Fact]
    public void Expand_UsesCameraOffEvent()
    {
        Assert.StartsWith("camera-off/", WebhookSender.Expand("{{event}}/{{timestamp}}", false));
    }
}
