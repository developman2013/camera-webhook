using System.Drawing;
using CameraWebhook.Models;
using CameraWebhook.Services;
using Forms = System.Windows.Forms;

namespace CameraWebhook;

public sealed class TrayController : IDisposable
{
    private readonly ConfigStore _store = new();
    private readonly WebhookSender _sender = new();
    private readonly Forms.NotifyIcon _icon;
    private CameraStateMonitor? _monitor;
    private AppConfig _config = new();
    private MainWindow? _window;

    public TrayController()
    {
        var menu = new Forms.ContextMenuStrip();
        menu.Items.Add("Настройки", null, (_, _) => ShowSettings());
        menu.Items.Add("Выход", null, (_, _) => System.Windows.Application.Current.Shutdown());
        _icon = new Forms.NotifyIcon
        {
            Text = "Camera Webhook",
            Icon = SystemIcons.Application,
            ContextMenuStrip = menu,
            Visible = true
        };
        _icon.DoubleClick += (_, _) => ShowSettings();
    }

    public void Start(bool background)
    {
        _config = _store.Load();
        ApplyConfig(_config);
        if (!background)
            ShowSettings();
    }

    private void ShowSettings()
    {
        if (_window is not null)
        {
            _window.Activate();
            return;
        }
        _window = new MainWindow(_config, _store.ConfigPath);
        _window.Saved += (_, config) =>
        {
            _store.Save(config);
            _config = config;
            ApplyConfig(config);
        };
        _window.Closed += (_, _) => _window = null;
        _window.Show();
        _window.Activate();
    }

    private void ApplyConfig(AppConfig config)
    {
        StartupManager.SetEnabled(config.StartWithWindows);
        _monitor?.Dispose();
        _monitor = new CameraStateMonitor(new CameraUsageDetector(), config.PollIntervalMilliseconds);
        _monitor.StateChanged += OnStateChanged;
        _monitor.Start();
    }

    private async void OnStateChanged(object? sender, bool cameraOn)
    {
        var text = cameraOn ? "Камера включена" : "Камера выключена";
        _icon.Text = $"Camera Webhook — {text}";
        try
        {
            await _sender.SendAsync(cameraOn ? _config.CameraOn : _config.CameraOff, cameraOn);
        }
        catch (Exception ex)
        {
            _icon.ShowBalloonTip(5000, "Camera Webhook", $"REST-запрос не выполнен: {ex.Message}", Forms.ToolTipIcon.Warning);
        }
    }

    public void Dispose()
    {
        _monitor?.Dispose();
        _sender.Dispose();
        _icon.Visible = false;
        _icon.Dispose();
    }
}
