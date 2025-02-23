Integrates Windows media metrics into MoBro.

# Windows 11 Disclaimer

This plugin **only works properly on Windows 10**!  

The issue arises from MoBro's architecture (parts of it run as a Windows service) and
a [permission change in Windows 11](https://learn.microsoft.com/en-us/answers/questions/1263190/can-not-access-globalsystemmediatransportcontrolss?comment=question#newest-question-comment),
which restricts access to UWP APIs in non-interactive sessions, such as services.

We have already planned a solution for this, but it involves a major architectural overhaul of how plugins are handled
in MoBro.  
Due to the complexity of this change, there is currently no estimated timeline for the fix.

---

# Setup

No additional setup is required.

---

# Metrics

This plugin provides the following metrics:

- Current media item's title
- Current media item's artist
- Master volume level
- Progress percentage of the current media item
- Total duration of the current media item
- Elapsed time of the current media item
- Remaining time of the current media item

---

# Settings

The plugin includes the following configurable settings:

| Setting | Default | Description                                          |
|---------|---------|------------------------------------------------------|
| Media   | enabled | Toggle metrics for the currently playing media item. |
| Audio   | enabled | Toggle general audio-related metrics.                |
