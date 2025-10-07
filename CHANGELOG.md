# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
