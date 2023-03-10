using MoBro.Plugin.Media.Helper;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models.Actions;
using MoBro.Plugin.SDK.Models.Metrics;
using MoBro.Plugin.SDK.Services;

namespace MoBro.Plugin.Media.Handlers;

public class AudioHandler : AbstractHandler
{
  public override IEnumerable<IMetric> GetMetrics()
  {
    yield return Metric(Ids.Metric.MVolume)
      .OfType(CoreMetricType.Usage)
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .AsDynamicValue()
      .Build();
  }

  public override IEnumerable<IAction> GetActions()
  {
    yield return Action(Ids.Action.MVolumeUp)
      .WithHandler(VolumeUp)
      .WithSetting(b => b
        .WithName("step_amount")
        .WithLabel("Step width")
        .OfTypeNumber()
        .WithDefault(1)
        .WithMin(1)
        .WithMax(100)
        .Build()
      )
      .Build();

    yield return Action(Ids.Action.MVolumeDown)
      .WithHandler(VolumeDown)
      .WithSetting(b => b
        .WithName("step_amount")
        .WithLabel("Step width")
        .OfTypeNumber()
        .WithDefault(1)
        .WithMin(1)
        .WithMax(100)
        .Build()
      )
      .Build();
  }

  public override async IAsyncEnumerable<MetricValue> GetMetricValues()
  {
    yield return Value(Ids.Metric.MVolume, (int)AudioManager.GetMasterVolume());
  }

  private static void VolumeUp(IMoBroSettings settings)
  {
    var stepAmount = Math.Abs(settings.GetValue("step_amount", 1));
    AudioManager.StepMasterVolume(stepAmount);
  }

  private static void VolumeDown(IMoBroSettings settings)
  {
    var stepAmount = Math.Abs(settings.GetValue("step_amount", 1));
    AudioManager.StepMasterVolume(-stepAmount);
  }
}