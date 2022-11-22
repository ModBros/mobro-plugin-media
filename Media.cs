using MoBro.Plugin.SDK;

namespace MoBro.Plugin.Media;

public sealed class Media : IMoBroPlugin
{
  private bool _paused;
  private IMoBroService _service;

  public void Init(IPluginSettings settings, IMoBroService service)
  {
    _service = service;

  }

  public void Pause() => _paused = true;

  public void Resume() => _paused = false;

  public void Dispose()
  {
  }
}