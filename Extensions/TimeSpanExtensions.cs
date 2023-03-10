namespace MoBro.Plugin.Media.Extensions;

public static class TimeSpanExtensions
{
  public static TimeSpan RoundUpToSecond(this TimeSpan timeSpan)
  {
    var ticks = timeSpan.Ticks + TimeSpan.TicksPerSecond - 1;
    var roundedTicks = ticks - ticks % TimeSpan.TicksPerSecond;
    return new TimeSpan(roundedTicks);
  }
}