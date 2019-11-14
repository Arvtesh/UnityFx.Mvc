# UnityFx.Mvc

Channel  | UnityFx.Mvc |
---------|---------------|
Github | [![GitHub release](https://img.shields.io/github/release/Arvtesh/UnityFx.Mvc.svg?logo=github)](https://github.com/Arvtesh/UnityFx.Mvc/releases)
Unity Asset Store | [![MVC framework for Unity](https://img.shields.io/badge/tools-v0.1.0-green.svg)](https://assetstore.unity.com/packages/tools/TODO)
Npm | [![Npm release](https://img.shields.io/npm/v/com.unityfx.mvc.svg)](https://www.npmjs.com/package/com.unityfx.mvc) ![npm](https://img.shields.io/npm/dt/com.unityfx.mvc)

## Synopsis

*UnityFx.Mvc* is an MVC(S) framework for Unity.

Please see [CHANGELOG](CHANGELOG.md) for information on recent changes.

## Getting Started
### Prerequisites
You may need the following software installed in order to build/use the library:
- [Unity3d 2018.4+](https://store.unity.com/).

### Getting the code
You can get the code by cloning the github repository using your preffered git client UI or you can do it from command line as follows:
```cmd
git clone https://github.com/Arvtesh/UnityFx.Mvc.git
```

### Npm package
[![NPM](https://nodei.co/npm/com.unityfx.mvc.png)](https://www.npmjs.com/package/com.unityfx.mvc)

Npm package is available at [npmjs.com](https://www.npmjs.com/package/com.unityfx.mvc). To use it, add the following line to dependencies section of your `manifest.json`. Unity should download and link the package automatically:
```json
{
  "scopedRegistries": [
    {
      "name": "Arvtesh",
      "url": "https://registry.npmjs.org/",
      "scopes": [
        "com.unityfx"
      ]
    }
  ],
  "dependencies": {
    "com.unityfx.mvc": "0.1.0"
  }
}
```

## Usage
Install the package and import the namespace:
```csharp
using UnityFx.Mvc;
```

## Understanding the concepts
TODO

## Motivation
The project was initially created to help author with his [Unity3d](https://unity3d.com) projects. Client .NET applications in general (and Unity applications specifically) do not have a standard structure or any kind of architecturing guidelines (like ASP.NET). This is an attempt to create a small yet effective and usable application framework suitable for Unity projects.

## Documentation
Please see the links below for extended information on the product:
- [Unity forums](https://forum.unity.com/threads/TODO/).
- [CHANGELOG](CHANGELOG.md).
- [SUPPORT](.github/SUPPORT.md).

## Useful links

## Software requirements
- [Microsoft Visual Studio](https://www.visualstudio.com/vs/community/)
- [Unity3d 2017+](https://store.unity.com/)

## Contributing
Please see [contributing guide](.github/CONTRIBUTING.md) for details.

## Versioning
The project uses [SemVer](https://semver.org/) versioning pattern. For the versions available, see [tags in this repository](https://github.com/Arvtesh/UnityFx.Mvc/tags).

## License
Please see the [![license](https://img.shields.io/github/license/Arvtesh/UnityFx.Mvc.svg)](LICENSE.md) for details.

## Acknowledgments
Working on this project is a great experience. Please see below list of sources of my inspiration (in no particular order):
* [ASP.NET](https://www.asp.net/). A great and well-designed framework.
* Everyone who ever commented or left any feedback on the project. It's always very helpful.

