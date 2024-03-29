using Windows.Media.Control;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models.Metrics;
using Action = MoBro.Plugin.SDK.Models.Actions.Action;

namespace MoBro.Plugin.Media.Handlers;

public class MediaHandler : AbstractHandler
{
  private GlobalSystemMediaTransportControlsSessionManager? _sm;
  private DateTime _lastPlaying;

  public MediaHandler()
  {
    _lastPlaying = DateTime.UtcNow;
  }

  public override IEnumerable<Metric> GetMetrics()
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

    // yield return Metric(Ids.Metric.Progress)
    //   .OfType(CoreMetricType.Usage)
    //   .OfCategory(CoreCategory.Media)
    //   .OfNoGroup()
    //   .AsDynamicValue()
    //   .Build();
    //
    // yield return Metric(Ids.Metric.DurationTotal)
    //   .OfType(CoreMetricType.Duration)
    //   .OfCategory(CoreCategory.Media)
    //   .OfNoGroup()
    //   .AsDynamicValue()
    //   .Build();
    //
    // yield return Metric(Ids.Metric.DurationPassed)
    //   .OfType(CoreMetricType.Duration)
    //   .OfCategory(CoreCategory.Media)
    //   .OfNoGroup()
    //   .AsDynamicValue()
    //   .Build();
    //
    // yield return Metric(Ids.Metric.DurationRemaining)
    //   .OfType(CoreMetricType.Duration)
    //   .OfCategory(CoreCategory.Media)
    //   .OfNoGroup()
    //   .AsDynamicValue()
    //   .Build();
  }

  public override IEnumerable<Action> GetActions()
  {
    yield return Action(Ids.Action.Play).WithAsyncHandler(Play).Build();
    yield return Action(Ids.Action.Pause).WithAsyncHandler(Pause).Build();
    yield return Action(Ids.Action.Next).WithAsyncHandler(Next).Build();
    yield return Action(Ids.Action.Previous).WithAsyncHandler(Previous).Build();
  }

  public override async IAsyncEnumerable<MetricValue> GetMetricValues()
  {
    var session = await GetSession();
    var mediaProps = session == null ? null : await session.TryGetMediaPropertiesAsync();

    // artist + title
    yield return Value(Ids.Metric.Title, mediaProps?.Title);
    yield return Value(Ids.Metric.Artist, mediaProps?.Artist);

    // var timelineProps = session?.GetTimelineProperties();
    // var playbackInfo = session?.GetPlaybackInfo();
    //
    // // progress values
    // if (timelineProps != null && playbackInfo != null)
    // {
    //   if (playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
    //   {
    //     _lastPlaying = DateTime.UtcNow;
    //   }
    //
    //   var passedPlayingTime = _lastPlaying - timelineProps.LastUpdatedTime.UtcDateTime;
    //   var totalTime = timelineProps.EndTime;
    //   var position = timelineProps.Position + (passedPlayingTime.Ticks > 0 ? passedPlayingTime : TimeSpan.Zero);
    //   var remaining = totalTime - position;
    //   var progress = position.TotalSeconds / totalTime.TotalSeconds;
    //
    //   // round all durations to full seconds 
    //   yield return Value(Ids.Metric.DurationPassed, position.RoundUpToSecond());
    //   yield return Value(Ids.Metric.DurationTotal, totalTime.RoundUpToSecond());
    //   yield return Value(Ids.Metric.DurationRemaining,
    //     remaining.Ticks > 0 ? remaining.RoundUpToSecond() : TimeSpan.Zero);
    //   yield return Value(Ids.Metric.Progress, Math.Min(100, Math.Ceiling(progress * 100)));
    // }
    // else
    // {
    //   yield return Value(Ids.Metric.DurationPassed, TimeSpan.Zero);
    //   yield return Value(Ids.Metric.DurationTotal, TimeSpan.Zero);
    //   yield return Value(Ids.Metric.DurationRemaining, TimeSpan.Zero);
    //   yield return Value(Ids.Metric.Progress, 0D);
    // }
  }

  private async Task Play()
  {
    var session = await GetSession();
    await session?.TryPlayAsync();
  }

  private async Task Pause()
  {
    var session = await GetSession();
    await session?.TryPauseAsync();
  }

  private async Task Next()
  {
    var session = await GetSession();
    await session?.TrySkipNextAsync();
  }

  private async Task Previous()
  {
    var session = await GetSession();
    await session?.TrySkipPreviousAsync();
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
}