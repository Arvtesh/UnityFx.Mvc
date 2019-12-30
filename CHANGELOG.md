# UnityFx.Mvc changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/); this project adheres to [Semantic Versioning](http://semver.org/).

## [0.2.0] - unreleased

### Added
- Added support for generic commands.
- Added view layers support (via `ViewControllerAttribute`).
- Added `IViewControllerResult` interface to tag controllers that have a result value.
- Added `IConfigurable` interfaces.
- Added message box extensions.

### Changed
- Changed the package layout. The code is now splitted into 3 assemblies (`UnityFx.Mvc`, `UnityFx.Mvc.Abstractions` and `UnityFx.Mvc.Extensions`).
- Renamed `IPresenter.PresentAsync` to `Present`. Added a group of `PresentAsync` extension methods returning `Task` instead of `IPresentResult`.
- Renamed `IPresentResult.DismissTask` to `Task`.
- Changed `Present`/`PresentAsync` arguments.

### Removed
- Removed `IPresentResult.PresentTask`.

## [0.1.0] - 2019.11.14

### Added
- Initial release.

