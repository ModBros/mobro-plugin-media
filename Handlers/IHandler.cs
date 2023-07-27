using MoBro.Plugin.SDK.Models.Metrics;
using Action = MoBro.Plugin.SDK.Models.Actions.Action;

namespace MoBro.Plugin.Media.Handlers;

public interface IHandler
{
  IEnumerable<Metric> GetMetrics();
  IEnumerable<Action> GetActions();
  IAsyncEnumerable<MetricValue> GetMetricValues();
}