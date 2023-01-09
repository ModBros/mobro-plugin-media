using System.Timers;
using MoBro.Plugin.Media.Helper;
using MoBro.Plugin.SDK;
using WindowsMediaController;
using Timer = System.Timers.Timer;

namespace MoBro.Plugin.Media;

public sealed class Media : IMoBroPlugin
{
  private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(500);

  private readonly IMoBroService _service;
  private readonly Timer _timer;
  private readonly MediaManager _mediaManager;
  private readonly MetricsHandler _metricsHandler;
  private readonly ActionsHandler _actionsHandler;

  public Media(IMoBroService service)
  {
    _service = service;
    _timer = new Timer
    {
      Interval = UpdateInterval.TotalMilliseconds,
      AutoReset = false,
      Enabled = false
    };
    _timer.Elapsed += OnTimer;

    _mediaManager = new MediaManager();
    _metricsHandler = new MetricsHandler(_mediaManager, service);
    _actionsHandler = new ActionsHandler(_mediaManager, service);
  }

  public void Init()
  {
    _mediaManager.Start();
    _metricsHandler.Start();
    _actionsHandler.Start();
    _timer.Start();
  }

  public void Pause() => _timer.Stop();

  public void Resume() => _timer.Start();

  private void OnTimer(object? sender, ElapsedEventArgs e)
  {
    try
    {
      _metricsHandler.UpdateValues().GetAwaiter().GetResult();
      _timer.Start();
    }
    catch (Exception exception)
    {
      _service.NotifyError(exception);
    }
  }

  public void Dispose()
  {
  }
}