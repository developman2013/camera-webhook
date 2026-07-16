using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using CameraWebhook.Models;

namespace CameraWebhook;

public partial class MainWindow : Window
{
    private readonly string _configPath;
    public event EventHandler<AppConfig>? Saved;

    public MainWindow(AppConfig config, string configPath)
    {
        InitializeComponent();
        _configPath = configPath;
        StartWithWindows.IsChecked = config.StartWithWindows;
        Fill(config.CameraOn, true);
        Fill(config.CameraOff, false);
    }

    private void Fill(EndpointConfig endpoint, bool cameraOn)
    {
        if (cameraOn)
        {
            OnEnabled.IsChecked = endpoint.Enabled;
            OnUrl.Text = endpoint.Url;
            OnMethod.Text = endpoint.Method;
            OnHeaders.Text = endpoint.HeadersJson;
            OnBody.Text = endpoint.Body;
        }
        else
        {
            OffEnabled.IsChecked = endpoint.Enabled;
            OffUrl.Text = endpoint.Url;
            OffMethod.Text = endpoint.Method;
            OffHeaders.Text = endpoint.HeadersJson;
            OffBody.Text = endpoint.Body;
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            ValidateHeaders(OnHeaders.Text);
            ValidateHeaders(OffHeaders.Text);
            var config = new AppConfig
            {
                StartWithWindows = StartWithWindows.IsChecked == true,
                CameraOn = ReadEndpoint(true),
                CameraOff = ReadEndpoint(false)
            };
            ValidateUrl(config.CameraOn);
            ValidateUrl(config.CameraOff);
            Saved?.Invoke(this, config);
            System.Windows.MessageBox.Show("Настройки сохранены.", "Camera Webhook", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message, "Некорректные настройки", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private EndpointConfig ReadEndpoint(bool cameraOn) => cameraOn
        ? new EndpointConfig { Enabled = OnEnabled.IsChecked == true, Url = OnUrl.Text.Trim(), Method = OnMethod.Text.Trim(), HeadersJson = OnHeaders.Text, Body = OnBody.Text }
        : new EndpointConfig { Enabled = OffEnabled.IsChecked == true, Url = OffUrl.Text.Trim(), Method = OffMethod.Text.Trim(), HeadersJson = OffHeaders.Text, Body = OffBody.Text };

    private static void ValidateHeaders(string json)
    {
        var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        if (headers is null)
            throw new InvalidOperationException("Заголовки должны быть JSON-объектом со строковыми значениями.");
    }

    private static void ValidateUrl(EndpointConfig endpoint)
    {
        if (endpoint.Enabled && (!Uri.TryCreate(endpoint.Url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)))
            throw new InvalidOperationException("Для включенного запроса укажите корректный HTTP(S) URL.");
    }

    private void OpenConfig_Click(object sender, RoutedEventArgs e)
    {
        var directory = Path.GetDirectoryName(_configPath)!;
        Directory.CreateDirectory(directory);
        Process.Start(new ProcessStartInfo("explorer.exe", directory) { UseShellExecute = true });
    }
}
