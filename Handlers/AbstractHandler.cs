using MoBro.Plugin.SDK.Builders;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models.Metrics;
using MoBro.Plugin.SDK.Models.Settings;
using MoBro.Plugin.SDK.Services;
using Action = MoBro.Plugin.SDK.Models.Actions.Action;

namespace MoBro.Plugin.Media.Handlers;

public abstract class AbstractHandler : IHandler
{
  public abstract IEnumerable<Metric> GetMetrics();
  public abstract IEnumerable<Action> GetActions();
  public abstract IAsyncEnumerable<MetricValue> GetMetricValues();

  protected static Metric Metric(string id, CoreMetricType metricType)
  {
    return MoBroItem
      .CreateMetric()
      .WithId(id)
      .WithLabel(id + "_label", id + "_desc")
      .OfType(metricType)
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .AsDynamicValue()
      .Build();
  }


  protected static Action Action(
    string id,
    Func<IMoBroSettings, Task> handler,
    params Func<SettingsBuilder.INameStage, SettingsFieldBase>[] settings
  )
  {
    var builder = MoBroItem
      .CreateAction()
      .WithId(id)
      .WithLabel(id + "_label", id + "_desc")
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .WithAsyncHandler(handler);

    foreach (var setting in settings)
    {
      builder.WithSetting(setting);
    }

    return builder.Build();
  }

  protected static MetricValue Value(string id, object? value) => new(id, value);
}