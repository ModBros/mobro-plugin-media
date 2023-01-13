using MoBro.Plugin.SDK.Builders;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models.Metrics;
using MoBro.Plugin.SDK.Services;
using WindowsMediaController;

namespace MoBro.Plugin.Media.Helper;

internal class MetricsHandler
{
  private readonly IDictionary<string, object?> _values;
  private readonly MediaManager _mediaManager;
  private readonly IMoBroService _service;

  public MetricsHandler(MediaManager mediaManager, IMoBroService service)
  {
    _mediaManager = mediaManager;
    _service = service;
    _values = new Dictionary<string, object?>();
  }

  public void Start()
  {
    _service.Register(GetMetrics());
  }

  public async Task UpdateValues()
  {
    _mediaManager.ForceUpdate();

    // master volume
    UpdateValue(Ids.Metric.MVolume, (int)AudioManager.GetMasterVolume());

    // artist + title
    var control = _mediaManager.GetFocusedSession().ControlSession;
    var properties = await control.TryGetMediaPropertiesAsync();
    UpdateValue(Ids.Metric.Title, properties.Title);
    UpdateValue(Ids.Metric.Artist, properties.Artist);

    // title progress
    // var timeline = control.GetTimelineProperties();
    // UpdateValue(Ids.Metric.Progress, 100 / timeline.EndTime.TotalSeconds * timeline.Position.TotalSeconds);
    // UpdateValue(Ids.Metric.Duration, timeline.EndTime);
    // UpdateValue(Ids.Metric.Position, timeline.Position);
    // UpdateValue(Ids.Metric.Remaining, timeline.EndTime - timeline.Position);
  }

  private void UpdateValue(string id, object? value)
  {
    if (_values.TryGetValue(id, out var storedVal) && Equals(storedVal, value)) return;

    _values[id] = value;
    _service.UpdateMetricValue(id, value);
  }

  private IEnumerable<IMetric> GetMetrics()
  {
    yield return MoBroItem
      .CreateMetric()
      .WithId(Ids.Metric.Title)
      .WithLabel("Title")
      .OfType(CoreMetricType.Text)
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .AsDynamicValue()
      .Build();

    yield return MoBroItem
      .CreateMetric()
      .WithId(Ids.Metric.Artist)
      .WithLabel("Artist")
      .OfType(CoreMetricType.Text)
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .AsDynamicValue()
      .Build();

    yield return MoBroItem
      .CreateMetric()
      .WithId(Ids.Metric.MVolume)
      .WithLabel("Volume")
      .OfType(CoreMetricType.Usage)
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .AsDynamicValue()
      .Build();

    // yield return MoBroItem
    //   .CreateMetric()
    //   .WithId(Ids.Metric.Progress)
    //   .WithLabel("Progress")
    //   .OfType(CoreMetricType.Usage)
    //   .OfCategory(CoreCategory.Media)
    //   .OfNoGroup()
    //   .AsDynamicValue()
    //   .Build();
    //
    // yield return MoBroItem
    //   .CreateMetric()
    //   .WithId(Ids.Metric.Duration)
    //   .WithLabel("Duration")
    //   .OfType(CoreMetricType.Duration)
    //   .OfCategory(CoreCategory.Media)
    //   .OfNoGroup()
    //   .AsDynamicValue()
    //   .Build();
    //
    // yield return MoBroItem
    //   .CreateMetric()
    //   .WithId(Ids.Metric.Position)
    //   .WithLabel("Position")
    //   .OfType(CoreMetricType.Duration)
    //   .OfCategory(CoreCategory.Media)
    //   .OfNoGroup()
    //   .AsDynamicValue()
    //   .Build();
    //
    // yield return MoBroItem
    //   .CreateMetric()
    //   .WithId(Ids.Metric.Remaining)
    //   .WithLabel("Remaining")
    //   .OfType(CoreMetricType.Duration)
    //   .OfCategory(CoreCategory.Media)
    //   .OfNoGroup()
    //   .AsDynamicValue()
    //   .Build();
  }
}