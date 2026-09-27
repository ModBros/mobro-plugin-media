using MoBro.Plugin.Media;
using MoBro.Plugin.SDK;
using Serilog.Events;

using var plugin = MoBroPluginBuilder
  .Create<Plugin>()
  .WithLogLevel(LogEventLevel.Debug)
  .WithSetting("s_title_placeholder", "Nothing playing")
  .Build();

Console.ReadLine();
