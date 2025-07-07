using Windows.Media.Control;
using MoBro.Plugin.Media.Helper;
using MoBro.Plugin.SDK.Builders;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models.Metrics;
using MoBro.Plugin.SDK.Models.Settings;
using MoBro.Plugin.SDK.Services;
using Action = MoBro.Plugin.SDK.Models.Actions.Action;

namespace MoBro.Plugin.Media.Handlers;

public class MediaHandler : AbstractHandler
{
  private GlobalSystemMediaTransportControlsSessionManager? _sm;

  public override IEnumerable<Metric> GetMetrics()
  {
    return
    [
      Metric(Ids.Metric.MasterVolume, CoreMetricType.Usage),
      Metric(Ids.Metric.Title, CoreMetricType.Text),
      Metric(Ids.Metric.Artist, CoreMetricType.Text),
      Metric(Ids.Metric.MasterMute, CoreMetricType.Boolean),
      Metric(Ids.Metric.Playing, CoreMetricType.Boolean)
    ];
  }

  public override IEnumerable<Action> GetActions()
  {
    Func<SettingsBuilder.INameStage, SettingsFieldBase> volumeStepAmountSetting =
      b => b.WithName(Ids.ActionSettings.VolumeStepAmount)
        .WithLabel(Ids.ActionSettings.VolumeStepAmount + "_label", Ids.ActionSettings.VolumeStepAmount + "_desc")
        .OfTypeNumber()
        .WithDefault(1)
        .WithMin(1)
        .WithMax(100)
        .Build();

    return
    [
      Action(Ids.Action.Play, Play),
      Action(Ids.Action.Pause, Pause),
      Action(Ids.Action.Next, Next),
      Action(Ids.Action.Previous, Previous),
      Action(Ids.Action.MasterVolumeUp, VolumeUp, Ids.Metric.MasterVolume, volumeStepAmountSetting),
      Action(Ids.Action.MaterVolumeDown, VolumeDown, Ids.Metric.MasterVolume, volumeStepAmountSetting),
      Action(Ids.Action.MasterMuteOn, MuteOn, Ids.Metric.MasterMute),
      Action(Ids.Action.MasterMuteOff, MuteOff, Ids.Metric.MasterMute),
      Action(Ids.Action.MasterMuteToggle, MuteToggle, Ids.Metric.MasterMute),
    ];
  }

  public override async IAsyncEnumerable<MetricValue> GetMetricValues()
  {
    // master volume
    yield return Value(Ids.Metric.MasterVolume, (int)AudioManager.GetMasterVolume());
    yield return Value(Ids.Metric.MasterMute, AudioManager.GetMasterVolumeMute());

    var session = await GetSession();
    var mediaProps = session == null ? null : await session.TryGetMediaPropertiesAsync();

    // artist + title
    yield return Value(Ids.Metric.Title, mediaProps?.Title);
    yield return Value(Ids.Metric.Artist, mediaProps?.Artist);
    
    // playing status
    var playbackInfo = session?.GetPlaybackInfo();
    var isPlaying = playbackInfo?.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;
    yield return Value(Ids.Metric.Playing, isPlaying);
  }

  private async Task Play(IMoBroSettings _)
  {
    var session = await GetSession();
    await session?.TryPlayAsync();
  }

  private async Task Pause(IMoBroSettings _)
  {
    var session = await GetSession();
    await session?.TryPauseAsync();
  }

  private async Task Next(IMoBroSettings _)
  {
    var session = await GetSession();
    await session?.TrySkipNextAsync();
  }

  private async Task Previous(IMoBroSettings _)
  {
    var session = await GetSession();
    await session?.TrySkipPreviousAsync();
  }

  private static Task VolumeUp(IMoBroSettings settings)
  {
    var stepAmount = Math.Abs(settings.GetValue(Ids.ActionSettings.VolumeStepAmount, 1));
    AudioManager.StepMasterVolume(stepAmount);
    return Task.CompletedTask;
  }

  private static Task VolumeDown(IMoBroSettings settings)
  {
    var stepAmount = Math.Abs(settings.GetValue(Ids.ActionSettings.VolumeStepAmount, 1));
    AudioManager.StepMasterVolume(-stepAmount);
    return Task.CompletedTask;
  }

  private static Task MuteToggle(IMoBroSettings arg)
  {
    AudioManager.ToggleMasterVolumeMute();
    return Task.CompletedTask;
  }

  private static Task MuteOff(IMoBroSettings arg)
  {
    AudioManager.SetMasterVolumeMute(false);
    return Task.CompletedTask;
  }

  private static Task MuteOn(IMoBroSettings arg)
  {
    AudioManager.SetMasterVolumeMute(true);
    return Task.CompletedTask;
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