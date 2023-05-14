using MoBro.Plugin.Media;
using MoBro.Plugin.SDK.Testing;
using Serilog.Events;

var plugin = MoBroPluginBuilder
  .Create<Media>()
  .WithLogLevel(LogEventLevel.Debug)
  .WithSetting("media", "true")
  .WithSetting("audio", "true")
  .Build();

Console.ReadLine();
