using MoBro.Plugin.SDK.Models.Actions;
using MoBro.Plugin.SDK.Models.Metrics;

namespace MoBro.Plugin.Media.Handlers;

public interface IHandler
{
  IEnumerable<IMetric> GetMetrics();
  IEnumerable<IAction> GetActions();
  IAsyncEnumerable<MetricValue> GetMetricValues();
}