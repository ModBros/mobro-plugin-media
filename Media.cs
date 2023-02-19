using MoBro.Plugin.Media.Handlers;
using MoBro.Plugin.SDK;
using MoBro.Plugin.SDK.Services;
using WindowsMediaController;

namespace MoBro.Plugin.Media;

public sealed class Media : IMoBroPlugin
{
  private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(1000);
  private static readonly TimeSpan InitialDelay = TimeSpan.FromSeconds(2);

  private readonly IMoBroService _service;
  private readonly IMoBroScheduler _scheduler;

  private readonly MediaManager _mediaManager;
  private readonly MetricsHandler _metricsHandler;
  // private readonly ActionsHandler _actionsHandler;

  public Media(IMoBroService service, IMoBroScheduler scheduler)
  {
    _service = service;
    _scheduler = scheduler;

    _mediaManager = new MediaManager();
    _metricsHandler = new MetricsHandler(_mediaManager, _service);
    // _actionsHandler = new ActionsHandler(_mediaManager, service);
  }

  public void Init()
  {
    _mediaManager.Start();
    _metricsHandler.Start();
    // _actionsHandler.Start();

    _scheduler.Interval(OnTimer, UpdateInterval, InitialDelay);
  }

  private void OnTimer()
  {
    _metricsHandler.UpdateValues().GetAwaiter().GetResult();
  }

  public void Dispose()
  {
  }
}