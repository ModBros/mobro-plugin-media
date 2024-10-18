# mobro-plugin-media

![GitHub tag (latest by date)](https://img.shields.io/github/v/tag/ModBros/mobro-plugin-media?label=version)
![GitHub](https://img.shields.io/github/license/ModBros/mobro-plugin-media)
[![MoBro](https://img.shields.io/badge/-MoBro-red.svg)](https://mobro.app)
[![Discord](https://img.shields.io/discord/620204412706750466.svg?color=7389D8&labelColor=6A7EC2&logo=discord&logoColor=ffffff&style=flat-square)](https://discord.com/invite/DSNX4ds)

**Media plugin for MoBro**

This plugin integrates Windows media metrics into [MoBro](https://mobro.app).

**Note**:  
Due to the architecture of MoBro (parts of it running as Windows service) and
a [permission change in Windows 11](https://learn.microsoft.com/en-us/answers/questions/1263190/can-not-access-globalsystemmediatransportcontrolss?comment=question#newest-question-comment)
(no access to UWP APIs when running in a non-interactive session, such as a service) this plugin currently only
functions properly in Windows 10.

## SDK

This plugin is built using the [MoBro Plugin SDK](https://github.com/ModBros/mobro-plugin-sdk).  
Developer documentation is available at [developer.mobro.app](https://developer.mobro.app).

---

Feel free to visit us on our [Discord](https://discord.com/invite/DSNX4ds) or [Forum](https://www.mod-bros.com/en/forum)
for any questions or in case you run into any issues.
