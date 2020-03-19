# UnityFx.Mvc changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/); this project adheres to [Semantic Versioning](http://semver.org/).

## [0.4.0] - unreleased

### Added

## [0.3.1] - 2020.03.19

### Added
- Added ability for `UGUIViewFactoryBuilder` to add multiple prefabs in a single call ([#13](https://github.com/Arvtesh/UnityFx.Mvc/issues/13)).

### Fixed
- Fixed coimpile error on Unity 2019.3+ ([#14](https://github.com/Arvtesh/UnityFx.Mvc/issues/14)).

## [0.3.0] - 2020.02.20

### Added
- Improved `Present` error handling ([#2](https://github.com/Arvtesh/UnityFx.Mvc/issues/2)).
- Added support for singleton controllers ([#4](https://github.com/Arvtesh/UnityFx.Mvc/issues/4)).
- Added support for controller tags ([#5](https://github.com/Arvtesh/UnityFx.Mvc/issues/5)).
- Added middleware support ([#7](https://github.com/Arvtesh/UnityFx.Mvc/issues/7)).
- Added default MessageBox implementation ([#9](https://github.com/Arvtesh/UnityFx.Mvc/issues/9)).
- Added support for PlayerLoop-based presenters ([#10](https://github.com/Arvtesh/UnityFx.Mvc/issues/10)).
- Added `PresenterBuilder` and `UGUIViewFactoryBuilder` as builder for new `IPresenter` and `IViewFactory` instances.
- Added `IPresentService` interface.

### Changed
- Made `IPresenter` and `IViewFactory` implementations internal to `UnityFx.Mvc` assembly ([#6](https://github.com/Arvtesh/UnityFx.Mvc/issues/6)). They are created via specialied builder classes.

### Removed
- Removed generic `Presenter<>` implementation.

## [0.2.1] - 2020.01.09

### Fixed
- Fixed IL2CPP compile error in `Presenter<>`.
- Fixed editor window to correctly detect selection in 2-column project window layout.

## [0.2.0] - 2020.01.05

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
- Changed `IViewFactory` and `IViewControllerFactory` interfaces.
- `IView` now does not inherit `IComponent`.

### Removed
- Removed `IPresentResult.PresentTask`.

## [0.1.0] - 2019.11.14

### Added
- Initial release.

