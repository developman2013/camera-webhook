using System.Net.Http;
using System.Text;
using System.Text.Json;
using CameraWebhook.Models;

namespace CameraWebhook.Services;

public sealed class WebhookSender : IDisposable
{
    private readonly HttpClient _client = new() { Timeout = TimeSpan.FromSeconds(15) };

    public async Task SendAsync(EndpointConfig endpoint, bool cameraOn, CancellationToken cancellationToken = default)
    {
        if (!endpoint.Enabled || !Uri.TryCreate(endpoint.Url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            return;

        var method = new HttpMethod(string.IsNullOrWhiteSpace(endpoint.Method) ? "POST" : endpoint.Method.Trim().ToUpperInvariant());
        using var request = new HttpRequestMessage(method, uri);
        if (!string.IsNullOrEmpty(endpoint.Body) && method != HttpMethod.Get && method != HttpMethod.Head)
            request.Content = new StringContent(Expand(endpoint.Body, cameraOn), Encoding.UTF8, "application/json");

        var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(endpoint.HeadersJson);
        if (headers is not null)
        {
            foreach (var (name, value) in headers)
            {
                var expanded = Expand(value, cameraOn);
                if (!request.Headers.TryAddWithoutValidation(name, expanded) && request.Content is not null)
                    request.Content.Headers.TryAddWithoutValidation(name, expanded);
            }
        }

        using var response = await _client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    internal static string Expand(string value, bool cameraOn)
    {
        return value
            .Replace("{{event}}", cameraOn ? "camera-on" : "camera-off", StringComparison.Ordinal)
            .Replace("{{timestamp}}", DateTimeOffset.UtcNow.ToString("O"), StringComparison.Ordinal)
            .Replace("{{machine}}", Environment.MachineName, StringComparison.Ordinal);
    }

    public void Dispose() => _client.Dispose();
}
