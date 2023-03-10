using MoBro.Plugin.Media.Handlers;
using MoBro.Plugin.SDK;
using MoBro.Plugin.SDK.Services;

namespace MoBro.Plugin.Media;

public sealed class Media : IMoBroPlugin
{
  private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(2000);
  private static readonly TimeSpan InitialDelay = TimeSpan.FromSeconds(2);

  private readonly IMoBroService _service;
  private readonly IMoBroScheduler _scheduler;
  private readonly IList<IHandler> _handlers;

  public Media(IMoBroService service, IMoBroScheduler scheduler, IMoBroSettings settings)
  {
    _service = service;
    _scheduler = scheduler;
    _handlers = new List<IHandler>();

    if (settings.GetValue("media", true)) _handlers.Add(new MediaHandler());
    if (settings.GetValue("audio", true)) _handlers.Add(new AudioHandler());
  }

  public void Init()
  {
    // no need to start scheduler if no handlers are enabled
    if (_handlers.Count <= 0) return;

    foreach (var handler in _handlers)
    {
      _service.Register(handler.GetMetrics());
      // _service.Register(handler.GetActions());
    }

    _scheduler.Interval(OnTimer, UpdateInterval, InitialDelay);
  }

  private void OnTimer()
  {
    Task.Run(async () =>
    {
      foreach (var handler in _handlers)
      {
        await foreach (var v in handler.GetMetricValues())
        {
          _service.UpdateMetricValue(v);
        }
      }
    }).Wait();
  }

  public void Dispose()
  {
  }
}