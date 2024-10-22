# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [5.4.0] - 2024-10-22

### Added

- added support for minified Unity applications

## [5.3.0] - 2024-08-23

### Added

- new values in [InseyeEyeTrackerAvailability](./Runtime/InseyeEyeTrackerAvailability.cs) enum

### Changed

- `GetTrackerAvailablility` method should throw exceptions less often

- proper exception is thrown when the library has missing or damaged AARs

- enriched `Readme.md`

- extended article about library installation

## [5.2.0] - 2024-07-23

### Added

- added information about maximum and minimum supported Android service to `InseyeAndroidSettings`

- new exception `SDKServiceToHigh` and `SDKServiceToLow` that are thrown when system service is incompatible with Unity SDK 

### Changed

- removed  `Readme.md.meta` and `LICENSE.meta`

## [5.1.0] - 2024-05-06

### Added

- new class [InseyeAndroidSettings](./Runtime/Android/InseyeAndroidSettings.cs) that allows changing service
  initialization timeout on Android device

## [5.0.7] - 2024-04-18

### Changed

- more logs added when debugging skd

- android libraries updated to version `0.0.7`

## [5.0.6] - 2024-04-12

### Changed

- aar update, fixed shared library

## [5.0.5] - 2024-04-12 [YANKED]

### Changed

- aar update

## [5.0.4] - 2024-01-24

### Changed

- android libraries updated to version 0.0.6

## [5.0.3] - 2023-11-28

### Added

- added CHANGELOG.md

### Changed

- android libraries updated to version 0.0.5

### Fixed

- fixed bug where application would crash if `Inseye.Interfaces.IGazeProvider` was not disposed on Android
