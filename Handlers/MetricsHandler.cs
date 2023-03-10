using Windows.Media.Control;
using MoBro.Plugin.Media.Helper;
using MoBro.Plugin.SDK.Builders;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models.Metrics;
using MoBro.Plugin.SDK.Services;

namespace MoBro.Plugin.Media.Handlers;

internal class MetricsHandler
{
  private readonly IMoBroService _service;

  private GlobalSystemMediaTransportControlsSessionManager _sm;
  private DateTime _lastPlaying;

  public MetricsHandler(IMoBroService service)
  {
    _service = service;
    _sm = GlobalSystemMediaTransportControlsSessionManager.RequestAsync().GetAwaiter().GetResult();
    _lastPlaying = DateTime.UtcNow;
  }

  public void Start()
  {
    _service.Register(GetMetrics());
  }

  public async Task UpdateValues()
  {
    // master volume
    _service.UpdateMetricValue(Ids.Metric.MVolume, (int)AudioManager.GetMasterVolume());

    var session = await GetSession();
    var mediaProps = session == null ? null : await session.TryGetMediaPropertiesAsync();
    var timelineProps = session?.GetTimelineProperties();
    var playbackInfo = session?.GetPlaybackInfo();

    // artist + title
    if (mediaProps != null)
    {
      _service.UpdateMetricValue(Ids.Metric.Title, mediaProps.Title);
      _service.UpdateMetricValue(Ids.Metric.Artist, mediaProps.Artist);
    }
    else
    {
      _service.UpdateMetricValue(Ids.Metric.Title, null);
      _service.UpdateMetricValue(Ids.Metric.Artist, null);
    }

    // progress values
    if (timelineProps != null && playbackInfo != null)
    {
      if (playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
      {
        _lastPlaying = DateTime.UtcNow;
      }

      var passedPlayingTime = _lastPlaying - timelineProps.LastUpdatedTime.UtcDateTime;
      var totalTime = timelineProps.EndTime;
      var position = timelineProps.Position + (passedPlayingTime.Ticks > 0 ? passedPlayingTime : TimeSpan.Zero);
      var remaining = totalTime - position;
      var progress = position.TotalSeconds / totalTime.TotalSeconds;

      _service.UpdateMetricValue(Ids.Metric.DurationPassed, position);
      _service.UpdateMetricValue(Ids.Metric.DurationTotal, totalTime);
      _service.UpdateMetricValue(Ids.Metric.DurationRemaining, remaining.Ticks > 0 ? remaining : TimeSpan.Zero);
      _service.UpdateMetricValue(Ids.Metric.Progress, Math.Min(100, Math.Ceiling(progress * 100)));
    }
    else
    {
      _service.UpdateMetricValue(Ids.Metric.DurationPassed, TimeSpan.Zero);
      _service.UpdateMetricValue(Ids.Metric.DurationTotal, TimeSpan.Zero);
      _service.UpdateMetricValue(Ids.Metric.DurationRemaining, TimeSpan.Zero);
      _service.UpdateMetricValue(Ids.Metric.Progress, 0D);
    }
  }

  private async Task<GlobalSystemMediaTransportControlsSession?> GetSession()
  {
    var session = _sm?.GetCurrentSession();
    if (session != null) return session;
    // the SessionManager seems to not pick up media sessions if it was created while there was no active session
    // so in this case we just recreate it
    _sm = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
    return _sm.GetCurrentSession();
  }

  private static IEnumerable<IMetric> GetMetrics()
  {
    yield return Metric(Ids.Metric.Title)
      .OfType(CoreMetricType.Text)
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .AsDynamicValue()
      .Build();

    yield return Metric(Ids.Metric.Artist)
      .OfType(CoreMetricType.Text)
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .AsDynamicValue()
      .Build();

    yield return Metric(Ids.Metric.MVolume)
      .OfType(CoreMetricType.Usage)
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .AsDynamicValue()
      .Build();

    yield return Metric(Ids.Metric.Progress)
      .OfType(CoreMetricType.Usage)
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .AsDynamicValue()
      .Build();

    yield return Metric(Ids.Metric.DurationTotal)
      .OfType(CoreMetricType.Duration)
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .AsDynamicValue()
      .Build();

    yield return Metric(Ids.Metric.DurationPassed)
      .OfType(CoreMetricType.Duration)
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .AsDynamicValue()
      .Build();

    yield return Metric(Ids.Metric.DurationRemaining)
      .OfType(CoreMetricType.Duration)
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .AsDynamicValue()
      .Build();
  }

  private static MetricBuilder.ITypeStage Metric(string id) => MoBroItem
    .CreateMetric()
    .WithId(id)
    .WithLabel(id + "_label", id + "_desc");
}