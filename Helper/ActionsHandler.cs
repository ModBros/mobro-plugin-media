using MoBro.Plugin.SDK;
using MoBro.Plugin.SDK.Builders;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models.Actions;
using WindowsMediaController;

namespace MoBro.Plugin.Media.Helper;

internal class ActionsHandler
{
  private readonly MediaManager _mediaManager;

  public ActionsHandler(MediaManager mediaManager, IMoBroService service)
  {
    _mediaManager = mediaManager;

    service.RegisterItems(GetActions());
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
      .Build();

    yield return MoBroItem
      .CreateAction()
      .WithId(Ids.Action.MVolumeDown)
      .WithLabel("Volume down")
      .OfCategory(CoreCategory.Media)
      .OfNoGroup()
      .WithHandler(VolumeDown)
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

  private void Play() => _mediaManager.GetFocusedSession().ControlSession.TryPlayAsync();

  private void Pause() => _mediaManager.GetFocusedSession().ControlSession.TryPauseAsync();

  private void Next() => _mediaManager.GetFocusedSession().ControlSession.TrySkipNextAsync();

  private void Previous() => _mediaManager.GetFocusedSession().ControlSession.TrySkipPreviousAsync();
}