namespace CameraWebhook.Services;

public sealed class CameraStateMonitor : IDisposable
{
    private readonly ICameraUsageDetector _detector;
    private readonly System.Threading.Timer _timer;
    private readonly int _intervalMilliseconds;
    private bool? _lastState;
    private int _checking;

    public event EventHandler<bool>? StateChanged;

    public CameraStateMonitor(ICameraUsageDetector detector, int intervalMilliseconds)
    {
        _detector = detector;
        _intervalMilliseconds = Math.Max(250, intervalMilliseconds);
        _timer = new System.Threading.Timer(Check, null, Timeout.Infinite, Timeout.Infinite);
    }

    public void Start()
    {
        _lastState = _detector.IsCameraInUse();
        _timer.Change(0, Timeout.Infinite);
    }

    private void Check(object? state)
    {
        if (Interlocked.Exchange(ref _checking, 1) != 0)
            return;
        try
        {
            var current = _detector.IsCameraInUse();
            if (_lastState is not null && current != _lastState)
                StateChanged?.Invoke(this, current);
            _lastState = current;
        }
        finally
        {
            Interlocked.Exchange(ref _checking, 0);
            _timer.Change(_intervalMilliseconds, Timeout.Infinite);
        }
    }

    public void Dispose() => _timer.Dispose();
}
