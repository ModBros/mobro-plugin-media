namespace MoBro.Plugin.Media;

internal static class Ids
{
  internal static class Metric
  {
    internal const string Title = "m_title";
    internal const string Artist = "m_artist";
    internal const string MasterVolume = "m_mvol";
    internal const string MasterMute = "m_mmute";
  }

  internal static class Action
  {
    internal const string MasterVolumeUp = "a_mvol_up";
    internal const string MaterVolumeDown = "a_mvol_down";
    internal const string Play = "a_play";
    internal const string Pause = "a_pause";
    internal const string Next = "a_next";
    internal const string Previous = "a_previous";
    internal const string MasterMuteOn = "a_mmute_on";
    internal const string MasterMuteOff = "a_mmute_off";
    internal const string MasterMuteToggle = "a_mmute_toggle";
  }

  internal static class ActionSettings
  {
    internal const string VolumeStepAmount = "s_volume_step_amount";
  }
}