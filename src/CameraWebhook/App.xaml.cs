using System.Threading;
using System.Windows;

namespace CameraWebhook;

public partial class App : System.Windows.Application
{
    private Mutex? _mutex;
    private TrayController? _controller;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _mutex = new Mutex(true, "CameraWebhook.SingleInstance", out var createdNew);
        if (!createdNew)
        {
            System.Windows.MessageBox.Show("Camera Webhook уже запущен.", "Camera Webhook");
            Shutdown();
            return;
        }

        _controller = new TrayController();
        _controller.Start(e.Args.Contains("--background", StringComparer.OrdinalIgnoreCase));
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _controller?.Dispose();
        _mutex?.Dispose();
        base.OnExit(e);
    }
}
