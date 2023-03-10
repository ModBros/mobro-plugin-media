namespace MoBro.Plugin.Media;

internal static class Ids
{
  internal static class Metric
  {
    internal const string Title = "m_title";
    internal const string Artist = "m_artist";
    internal const string MVolume = "m_mvol";
    internal const string Progress = "m_progress";
    internal const string DurationTotal = "m_duration_total";
    internal const string DurationPassed = "m_duration_passed";
    internal const string DurationRemaining = "m_duration_remaining";
  }

  internal static class Action
  {
    internal const string MVolumeUp = "a_mvol_up";
    internal const string MVolumeDown = "a_mvol_down";
    internal const string Play = "a_play";
    internal const string Pause = "a_pause";
    internal const string Next = "a_next";
    internal const string Previous = "a_previous";
  }
}