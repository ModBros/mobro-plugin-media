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

    // due to a permission change in windows 11 the media metrics only work in windows 10 
    // the plugin is running under the SYSTEM user which no longer has access to UWP APIs including the
    // GlobalSystemMediaTransportControlsSessionManager used to read the media metrics
    // https://learn.microsoft.com/en-us/answers/questions/1263190/can-not-access-globalsystemmediatransportcontrolss?comment=question#newest-question-comment
    if (!IsWin11() && settings.GetValue("media", true)) _handlers.Add(new MediaHandler());
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

  private static bool IsWin11()
  {
    return Environment.OSVersion.Version >= new Version(10, 0, 22000, 0);
  }

  public void Dispose()
  {
  }
}