# Deprecated project

This project is deprecated and should not be used by new projects.

# Distancify.LitiumAddOns.Foundation

[![Build status](https://ci.appveyor.com/api/projects/status/0lyu8fx7s67is8wv?svg=true)](https://ci.appveyor.com/project/KristofferLindvall/distancify-litiumaddons-foundation)

This project contains a couple of abstractions and implementations that are commonly used among all our Litium Add-Ons.

## Litium Compatibility Chart

| Library Version | Targeted Litium Version |
| --------------- | ----------------------- |
| 1.x             | 6.x                     |
| 2.x             | 7.x                     |

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Install

```
Install-Package Distancify.LitiumAddOns.Foundation
```

### Prerequisites

This library aims at extending the e-comemrce platform [Litium](https://www.litium.se/). In order to build and develop the project, you need to fulfill their [development system requirements](https://docs.litium.com/documentation/get-started/system-requirements#DevEnv).

### Publishing

The project is built on AppVeyor and set up to automatically push any release branch to NuGet.org. To create a new release, create a new branch using the following convention: `release/v<Major>.<Minor>`. AppVeyor will automatically append the build number.

## Running the tests

The tests are built using xUnit and does not require any setup in order to run inside Visual Studio's standard test runner.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning.

## Authors

See the list of [contributors](https://github.com/distancify/Distancify.LitiumAddOns.Foundation/graphs/contributors) who participated in this project.

## License

This project is licensed under the LGPL v3 License - see the [LICENSE](LICENSE) file for details