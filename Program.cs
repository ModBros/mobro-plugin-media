using MoBro.Plugin.Media;
using MoBro.Plugin.SDK;
using Serilog.Events;

using var plugin = MoBroPluginBuilder
  .Create<Plugin>()
  .WithLogLevel(LogEventLevel.Debug)
  .WithSetting("media", "true")
  .WithSetting("audio", "true")
  .Build();

Console.ReadLine();
