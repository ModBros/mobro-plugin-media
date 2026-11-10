using Microsoft.Extensions.Logging;
using Windows.Media.Control;
using MoBro.Plugin.Media.Helper;
using MoBro.Plugin.SDK.Builders;
using MoBro.Plugin.SDK.Enums;
using MoBro.Plugin.SDK.Models.Metrics;
using MoBro.Plugin.SDK.Models.Settings;
using MoBro.Plugin.SDK.Services;
using Action = MoBro.Plugin.SDK.Models.Actions.Action;

namespace MoBro.Plugin.Media.Handlers;

public class MediaHandler(IMoBroService service, IMoBroSettings settings, ILogger logger) : AbstractHandler, IDisposable
{
  private readonly string _titlePlaceHolder = settings.GetValue<string>(Ids.Setting.TitlePlaceholder, "");

  private readonly Lock _lock = new();
  private GlobalSystemMediaTransportControlsSessionManager? _sm;
  private GlobalSystemMediaTransportControlsSession? _session;
  private bool _disposed;

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
      Action(Ids.Action.MasterVolumeDown, VolumeDown, Ids.Metric.MasterVolume, Ids.Group.Volume,
        volumeStepAmountSetting),
      Action(Ids.Action.MasterMuteOn, MuteOn, Ids.Metric.MasterMute, Ids.Group.Volume),
      Action(Ids.Action.MasterMuteOff, MuteOff, Ids.Metric.MasterMute, Ids.Group.Volume),
      Action(Ids.Action.MasterMuteToggle, MuteToggle, Ids.Metric.MasterMute, Ids.Group.Volume),
    ];
  }

  public override async IAsyncEnumerable<MetricValue> GetMetricValues()
  {
    // master volume
    yield return Value(Ids.Metric.MasterVolume, GetMasterVolume());
    yield return Value(Ids.Metric.MasterMute, GetMasterVolumeMute());

    var session = await GetSession();
    foreach (var value in await GetMediaValues(session))
    {
      yield return value;
    }
  }

  private async Task<MetricValue[]> GetMediaValues(GlobalSystemMediaTransportControlsSession? session)
  {
    var mediaProps = session == null ? null : await session.TryGetMediaPropertiesAsync();
    var playbackInfo = session?.GetPlaybackInfo();
    var isPlaying = playbackInfo?.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;

    return
    [
      // artist + title
      Value(Ids.Metric.Title, mediaProps?.Title ?? _titlePlaceHolder),
      Value(Ids.Metric.Artist, mediaProps?.Artist ?? ""),
      // playing status
      Value(Ids.Metric.Playing, isPlaying)
    ];
  }

  private async Task Play(IMoBroSettings _)
  {
    if (await GetSession() is not { } session) return;
    if (await session.TryPlayAsync())
    {
      service.UpdateMetricValue(Ids.Metric.Playing, true);
    }
  }

  private async Task Pause(IMoBroSettings _)
  {
    if (await GetSession() is not { } session) return;
    if (await session.TryPauseAsync())
    {
      service.UpdateMetricValue(Ids.Metric.Playing, false);
    }
  }

  private async Task Next(IMoBroSettings _)
  {
    if (await GetSession() is { } session) await session.TrySkipNextAsync();
  }

  private async Task Previous(IMoBroSettings _)
  {
    if (await GetSession() is { } session) await session.TrySkipPreviousAsync();
  }

  private Task VolumeUp(IMoBroSettings settings)
  {
    var stepAmount = Math.Abs(settings.GetValue(Ids.ActionSettings.VolumeStepAmount, 1));
    var volume = AudioManager.StepMasterVolume(stepAmount);
    service.UpdateMetricValue(Ids.Metric.MasterVolume, ToVolumeValue(volume));
    return Task.CompletedTask;
  }

  private Task VolumeDown(IMoBroSettings settings)
  {
    var stepAmount = Math.Abs(settings.GetValue(Ids.ActionSettings.VolumeStepAmount, 1));
    var volume = AudioManager.StepMasterVolume(-stepAmount);
    service.UpdateMetricValue(Ids.Metric.MasterVolume, ToVolumeValue(volume));
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

  private int? GetMasterVolume()
  {
    try
    {
      return ToVolumeValue(AudioManager.GetMasterVolume());
    }
    catch (Exception e)
    {
      // e.g. no audio output device available
      logger.LogDebug(e, "Failed to read master volume");
      return null;
    }
  }

  private bool? GetMasterVolumeMute()
  {
    try
    {
      return AudioManager.GetMasterVolumeMute();
    }
    catch (Exception e)
    {
      // e.g. no audio output device available
      logger.LogDebug(e, "Failed to read master volume mute state");
      return null;
    }
  }

  // AudioManager returns -1 on error
  private static int? ToVolumeValue(float volume) => volume < 0 ? null : (int)Math.Round(volume);

  private async Task<GlobalSystemMediaTransportControlsSession?> GetSession()
  {
    var session = _sm?.GetCurrentSession();
    if (session == null)
    {
      // the SessionManager seems to not pick up media sessions if it was created while there was no active session
      // so in this case we just recreate it
      var sm = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
      SetSessionManager(sm);
      session = sm.GetCurrentSession();
    }

    SetSession(session);
    return session;
  }

  // subscribe to change events to push updates immediately instead of waiting for the next poll
  private void SetSessionManager(GlobalSystemMediaTransportControlsSessionManager sm)
  {
    lock (_lock)
    {
      // a poll or action may still be running after shutdown, don't subscribe again
      if (_disposed) return;
      if (_sm != null) _sm.CurrentSessionChanged -= OnCurrentSessionChanged;
      _sm = sm;
      _sm.CurrentSessionChanged += OnCurrentSessionChanged;
    }
  }

  private void SetSession(GlobalSystemMediaTransportControlsSession? session)
  {
    lock (_lock)
    {
      if (_disposed) session = null;
      if (ReferenceEquals(_session, session)) return;

      if (_session != null)
      {
        _session.MediaPropertiesChanged -= OnMediaPropertiesChanged;
        _session.PlaybackInfoChanged -= OnPlaybackInfoChanged;
      }

      _session = session;

      if (_session != null)
      {
        _session.MediaPropertiesChanged += OnMediaPropertiesChanged;
        _session.PlaybackInfoChanged += OnPlaybackInfoChanged;
      }
    }
  }

  // unsubscribe from all change events, Windows would otherwise keep calling this handler after shutdown
  public void Dispose()
  {
    lock (_lock)
    {
      _disposed = true;
      if (_sm != null) _sm.CurrentSessionChanged -= OnCurrentSessionChanged;
    }

    SetSession(null);
  }

  private void OnCurrentSessionChanged(
    GlobalSystemMediaTransportControlsSessionManager sender,
    CurrentSessionChangedEventArgs args
  )
  {
    var session = sender.GetCurrentSession();
    SetSession(session);
    PushMediaValues(session);
  }

  private void OnMediaPropertiesChanged(
    GlobalSystemMediaTransportControlsSession sender,
    MediaPropertiesChangedEventArgs args
  ) => PushMediaValues(sender);

  private void OnPlaybackInfoChanged(
    GlobalSystemMediaTransportControlsSession sender,
    PlaybackInfoChangedEventArgs args
  ) => PushMediaValues(sender);

  private async void PushMediaValues(GlobalSystemMediaTransportControlsSession? session)
  {
    try
    {
      // ignore late events of a session that is no longer the current one
      if (!ReferenceEquals(session, _session)) return;

      foreach (var value in await GetMediaValues(session))
      {
        service.UpdateMetricValue(value);
      }
    }
    catch (Exception e)
    {
      logger.LogDebug(e, "Failed to push media metric values");
    }
  }
}