using Windows.Media.Control;
using MoBro.Plugin.Media.Helper;
using MoBro.Plugin.SDK.Builders;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models.Metrics;
using MoBro.Plugin.SDK.Models.Settings;
using MoBro.Plugin.SDK.Services;
using Action = MoBro.Plugin.SDK.Models.Actions.Action;

namespace MoBro.Plugin.Media.Handlers;

public class MediaHandler(IMoBroService service, IMoBroSettings settings) : AbstractHandler
{
  private readonly string _titlePlaceHolder = settings.GetValue<string>(Ids.Setting.TitlePlaceholder, "");

  private GlobalSystemMediaTransportControlsSessionManager? _sm;

  public override IEnumerable<Metric> GetMetrics()
  {
    return
    [
      Metric(Ids.Metric.Title, CoreMetricType.Text, Ids.Group.NowPlaying),
      Metric(Ids.Metric.Artist, CoreMetricType.Text, Ids.Group.NowPlaying),
      Metric(Ids.Metric.Playing, CoreMetricType.Boolean, Ids.Group.NowPlaying),
      Metric(Ids.Metric.MasterVolume, CoreMetricType.Usage, Ids.Group.Volume),
      Metric(Ids.Metric.MasterMute, CoreMetricType.Boolean, Ids.Group.Volume)
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
      Action(Ids.Action.Play, Play, groupId: Ids.Group.NowPlaying),
      Action(Ids.Action.Pause, Pause, groupId: Ids.Group.NowPlaying),
      Action(Ids.Action.Next, Next, groupId: Ids.Group.NowPlaying),
      Action(Ids.Action.Previous, Previous, groupId: Ids.Group.NowPlaying),
      Action(Ids.Action.MasterVolumeUp, VolumeUp, Ids.Metric.MasterVolume, Ids.Group.Volume, volumeStepAmountSetting),
      Action(Ids.Action.MaterVolumeDown, VolumeDown, Ids.Metric.MasterVolume, Ids.Group.Volume,
        volumeStepAmountSetting),
      Action(Ids.Action.MasterMuteOn, MuteOn, Ids.Metric.MasterMute, Ids.Group.Volume),
      Action(Ids.Action.MasterMuteOff, MuteOff, Ids.Metric.MasterMute, Ids.Group.Volume),
      Action(Ids.Action.MasterMuteToggle, MuteToggle, Ids.Metric.MasterMute, Ids.Group.Volume),
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
    yield return Value(Ids.Metric.Title, mediaProps?.Title ?? _titlePlaceHolder);
    yield return Value(Ids.Metric.Artist, mediaProps?.Artist ?? "");

    // playing status
    var playbackInfo = session?.GetPlaybackInfo();
    var isPlaying = playbackInfo?.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;
    yield return Value(Ids.Metric.Playing, isPlaying);
  }

  private async Task Play(IMoBroSettings _)
  {
    var session = await GetSession();
    var success = await session?.TryPlayAsync();
    if (success)
    {
      service.UpdateMetricValue(Ids.Metric.Playing, true);
    }
  }

  private async Task Pause(IMoBroSettings _)
  {
    var session = await GetSession();
    var success = await session?.TryPauseAsync();
    if (success)
    {
      service.UpdateMetricValue(Ids.Metric.Playing, false);
    }
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

  private Task VolumeUp(IMoBroSettings settings)
  {
    var stepAmount = Math.Abs(settings.GetValue(Ids.ActionSettings.VolumeStepAmount, 1));
    var volume = AudioManager.StepMasterVolume(stepAmount);
    service.UpdateMetricValue(Ids.Metric.MasterVolume, (int)volume);
    return Task.CompletedTask;
  }

  private Task VolumeDown(IMoBroSettings settings)
  {
    var stepAmount = Math.Abs(settings.GetValue(Ids.ActionSettings.VolumeStepAmount, 1));
    var volume = AudioManager.StepMasterVolume(-stepAmount);
    service.UpdateMetricValue(Ids.Metric.MasterVolume, (int)volume);
    return Task.CompletedTask;
  }

  private Task MuteToggle(IMoBroSettings arg)
  {
    var mute = AudioManager.ToggleMasterVolumeMute();
    service.UpdateMetricValue(Ids.Metric.MasterMute, mute);
    return Task.CompletedTask;
  }

  private Task MuteOff(IMoBroSettings arg)
  {
    AudioManager.SetMasterVolumeMute(false);
    service.UpdateMetricValue(Ids.Metric.MasterMute, false);
    return Task.CompletedTask;
  }

  private Task MuteOn(IMoBroSettings arg)
  {
    AudioManager.SetMasterVolumeMute(true);
    service.UpdateMetricValue(Ids.Metric.MasterMute, true);
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