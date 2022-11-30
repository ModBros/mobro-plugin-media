using System.Timers;
using MoBro.Plugin.Media.Helper;
using MoBro.Plugin.SDK;
using WindowsMediaController;
using Timer = System.Timers.Timer;

namespace MoBro.Plugin.Media;

public sealed class Media : IMoBroPlugin
{
  private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(500);

  private readonly Timer _timer;
  private readonly MediaManager _mediaManager;

  private bool _paused;

  private MetricsHandler _metricsHandler;
  private ActionsHandler _actionsHandler;

  public Media()
  {
    _timer = new Timer
    {
      Interval = UpdateInterval.TotalMilliseconds,
      AutoReset = false,
      Enabled = false
    };
    _timer.Elapsed += OnTimer;

    _mediaManager = new MediaManager();
    _mediaManager.StartAsync();
  }

  public void Init(IMoBroSettings settings, IMoBroService service)
  {
    _metricsHandler = new MetricsHandler(_mediaManager, service);
    _actionsHandler = new ActionsHandler(_mediaManager, service);

    _timer.Start();
  }

  public void Pause()
  {
    _paused = true;
    _timer.Stop();
  }

  public void Resume()
  {
    _paused = false;
    _timer.Start();
  }

  private void OnTimer(object? sender, ElapsedEventArgs e)
  {
    if (_paused) return;
    _metricsHandler.UpdateValues().GetAwaiter().GetResult();
    _timer.Start();
  }

  public void Dispose()
  {
  }
}