using System.Diagnostics;
using MoBro.Plugin.Media.Helper;
using MoBro.Plugin.SDK.Builders;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models.Actions;
using MoBro.Plugin.SDK.Services;

namespace MoBro.Plugin.Media.Handlers;

internal class ActionsHandler
{
  private readonly IMoBroService _service;

  public ActionsHandler(IMoBroService service)
  {
    _service = service;
  }

  public void Start()
  {
    _service.Register(GetActions());
  }

  private IEnumerable<IAction> GetActions()
  {
    yield return MoBroItem
      .CreateAction()
      .WithId(Ids.Action.MVolumeUp)
      .WithLabel("Volume up")
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
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

    yield return MoBroItem
      .CreateAction()
      .WithId(Ids.Action.MVolumeDown)
      .WithLabel("Volume down")
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
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

    yield return MoBroItem
      .CreateAction()
      .WithId(Ids.Action.Play)
      .WithLabel("Play")
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .WithHandler(Play)
      .Build();

    yield return MoBroItem
      .CreateAction()
      .WithId(Ids.Action.Pause)
      .WithLabel("Pause")
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .WithHandler(Pause)
      .Build();

    yield return MoBroItem
      .CreateAction()
      .WithId(Ids.Action.Next)
      .WithLabel("Next")
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .WithHandler(Next)
      .Build();

    yield return MoBroItem
      .CreateAction()
      .WithId(Ids.Action.Previous)
      .WithLabel("Previous")
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .WithHandler(Previous)
      .Build();
  }

  private void VolumeUp(IMoBroSettings settings)
  {
    var stepAmount = Math.Abs(settings.GetValue("step_amount", 1));
    AudioManager.StepMasterVolume(stepAmount);
  }

  private void VolumeDown(IMoBroSettings settings)
  {
    var stepAmount = Math.Abs(settings.GetValue("step_amount", 1));
    AudioManager.StepMasterVolume(-stepAmount);
  }

  private void Play()
  {
    // TODO
  }

  private void Pause()
  {
    // TODO
  }

  private void Next()
  {
    // TODO
  }

  private void Previous()
  {
    // TODO
  }
}