# UnityFx.Mvc

Channel  | UnityFx.Mvc |
---------|---------------|
Github | [![GitHub release](https://img.shields.io/github/release/Arvtesh/UnityFx.Mvc.svg?logo=github)](https://github.com/Arvtesh/UnityFx.Mvc/releases)
Unity Asset Store | [![MVC framework for Unity](https://img.shields.io/badge/tools-v0.2.0-green.svg)](https://assetstore.unity.com/packages/tools/TODO)
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
    "com.unityfx.mvc": "0.2.0"
  }
}
```

## Understanding the concepts
As outlined in [ASP.NET Core documentation](https://docs.microsoft.com/en-us/aspnet/core/mvc/overview), the Model-View-Controller (MVC) architectural pattern separates an application into three main groups of components: Models, Views, and Controllers. This pattern helps to achieve separation of concerns. Using this pattern, user requests are routed to a Controller which is responsible for working with the Model to perform user actions and/or retrieve results of queries. The Controller chooses the View to display to the user, and provides it with any Model data it requires.

This delineation of responsibilities helps you scale the application in terms of complexity because it's easier to code, debug, and test something (model, view, or controller) that has a single job. It's more difficult to update, test, and debug code that has dependencies spread across two or more of these three areas. For example, user interface logic tends to change more frequently than business logic. If presentation code and business logic are combined in a single object, an object containing business logic must be modified every time the user interface is changed. This often introduces errors and requires the retesting of business logic after every minimal user interface change.

Both the view and the controller depend on the model. However, the model depends on neither the view nor the controller. This is one of the key benefits of the separation. This separation allows the model to be built and tested independent of the visual presentation.

In the same way that MVC takes the position that you should separate model logic from view and controller logic, MVCS takes this notion a step further by advocating for application logic to live in the services. This is the recommended way of sharing pieces of logic between controllers.

### Model
The Model in an MVC application represents the state of the application and any business logic or operations that should be performed by it. Business logic should be encapsulated in the model, along with any implementation logic for persisting the state of the application.

### View
Views are responsible for presenting content through the user interface. There should be minimal logic within views, and any logic in them should relate to presenting content.

### Controller
Controllers are the components that handle user interaction, work with the model. In an MVC application, the view only displays information; the controller handles and responds to user input and interaction.

### Service
Services usually contian very specialized logic, that is shared between several controllers. Services may on may not depend on Model. Neither model not views should not depend on services. Examples of services: `IFileSystem`, `IAssetFactory`, `IAudioService`.

## Usage
Install the package and import the namespace:
```csharp
using UnityFx.Mvc;
using UnityFx.Mvc.Extensions;
```



## Motivation
The project was initially created to help author with his [Unity3d](https://unity3d.com) projects. Client .NET applications in general (and Unity applications specifically) do not have a standard structure or any kind of architecturing guidelines (like ASP.NET). This is an attempt to create a small yet effective and usable application framework suitable for Unity projects.

## Documentation
Please see the links below for extended information on the product:
- [Unity forums](https://forum.unity.com/threads/TODO/).
- [CHANGELOG](CHANGELOG.md).
- [SUPPORT](.github/SUPPORT.md).

## Useful links
- [MVC in Wikipedia](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller).
- [MVC in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/mvc/overview).
- [Architectural pronciples](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles).

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

