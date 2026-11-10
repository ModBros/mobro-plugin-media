# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 1.2.2 - 2026-11-10

### Changed

- Updated MoBro SDK to v1.2.0
- Title, artist and playing status update immediately when the media changes instead of up to 2 seconds later

### Fixed

- Play, pause, next and previous actions no longer fail when no media session is active
- Title, artist and playing status keep updating when no audio output device is available
- Master volume no longer reports -1 % when it cannot be read
- Master volume is rounded instead of truncated (e.g. no longer shows 29 % instead of 30 %)
- Media change events are unsubscribed when the plugin shuts down

## 1.2.1 - 2026-06-09

### Changed

- Properly organize metrics and actions in groups

## 1.2.0 - 2026-03-01

### Added

- Configurable title placeholder text when no media is playing, defaults to empty string.

### Changed

- Updated to .NET 10
- Updated to SDK v1.1.1

## 1.1.0 - 2025-10-07

### Added

- New boolean metric indicating whether media is currently playing
- Directly update the affected metric value after action invocation

## 1.0.0 - 2025-06-09

### Added

- Step amount setting for volume control actions
- New actions:
    - Mute Master
    - Unmute Master
    - Toggle mute for Master
- New metrics:
    - Current master mute status

### Changed

- Updated MoBro SDK to v1.0.2
- Updated to .NET 8
- General refactoring
- Removed plugin settings

### Fixed

- Fixed Windows 11 permission issue by running in the user session

## 0.0.3 - 2023-07-27

### Changed

- Updated SDK

## 0.0.2 - 2023-07-03

### Added

- Added Program.cs to run and test the plugin locally

### Changed

- Updated SDK
- Do not publish .dll of SDK
- Skip media metrics on Windows 11 (currently not supported)

## 0.0.1 - 2023-03-15

### Added

- Initial release
