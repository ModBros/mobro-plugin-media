using MoBro.Plugin.SDK.Builders;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models.Metrics;
using Action = MoBro.Plugin.SDK.Models.Actions.Action;

namespace MoBro.Plugin.Media.Handlers;

public abstract class AbstractHandler : IHandler
{
  public abstract IEnumerable<Metric> GetMetrics();
  public abstract IEnumerable<Action> GetActions();
  public abstract IAsyncEnumerable<MetricValue> GetMetricValues();

  protected static MetricBuilder.ITypeStage Metric(string id) => MoBroItem
    .CreateMetric()
    .WithId(id)
    .WithLabel(id + "_label", id + "_desc");

  protected static ActionBuilder.IHandlerStage Action(string id) => MoBroItem
    .CreateAction()
    .WithId(id)
    .WithLabel(id + "_label", id + "_desc")
    .OfCategory(CoreCategory.Media)
    .OfNoGroup();

  protected static MetricValue Value(string id, object? value) => new(id, value);
}