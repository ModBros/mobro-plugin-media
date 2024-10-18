Integrates Windows media metrics into MoBro.

# Windows 11 disclaimer

This plugin currently **only function properly in Windows 10**!

This issue is a result of the architecture of MoBro (parts of it running as Windows service) and
a [permission change in Windows 11](https://learn.microsoft.com/en-us/answers/questions/1263190/can-not-access-globalsystemmediatransportcontrolss?comment=question#newest-question-comment) (
no access to UWP APIs when running in a non-interactive session, such as a service).

We already have plans on how to fix this, but it will require a major architectural change on how plugins are handled in
MoBro.  
As this is a considerable amount of work, there is currently no ETA on when this fix will be implemented.

# Setup

No further setup required.

# Metrics

Provides the following metrics:

- Title of the current media item
- Artist of the current media item
- Current master volume
- Percentage progress of the current media item
- Total duration of the current media item
- Already passed duration of the current media item
- Remaining duration of the current media item

# Settings

This plugin exposes the following settings:

| Setting | Default | Explanation                                                     |
|---------|---------|-----------------------------------------------------------------|
| Media   | enabled | Whether to include metrics on the currently running media item. |
| Audio   | enabled | Whether to include general audio metrics.                       |

