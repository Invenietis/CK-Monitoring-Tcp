# CK-Monitoring TCP GrandOutput sender and receiver

GrandOutput handler (client) and receiver (server) using [CK.ControlChannel.Tcp](https://github.com/Invenietis/CK-ControlChannel-Tcp) client and server packages, for CK-Monitoring.

## Build requirements

- Windows
- Powershell
- [.NET Core SDK 1.0.4](https://www.microsoft.com/net/download/core) (with .NET Core 1.1)
- [Visual Studio 2017](https://www.visualstudio.com/) (any edition) with .NET framework build tools

## Build instructions

1. Clone the repository
2. In Powershell, run `CodeCakeBuilder/Bootstrap.ps1`
3. Run `CodeCakeBuilder/bin/Release/CodeCakeBuilder.exe`

## NuGet packages

| Feed             | Server | Client | Common |
| ---------------- | ------ | ------ | ------ |
| **NuGet stable** | (TBD)  | (TBD)  | (TBD)  |
| NuGet prerelease | (TBD)  | (TBD)  | (TBD)  |
| MyGet preview    | (TBD)  | (TBD)  | (TBD)  |
| MyGet CI         | (TBD)  | (TBD)  | (TBD)  |

## Build status

| Branch     | Visual Studio 2017 |
| ---------- | ------------------ |
| *(latest)* | (TBD)              |
| `master`   | (TBD)              |
| `develop`  | (TBD)              |

## Contributing

Anyone and everyone is welcome to contribute. Please take a moment to
review the [guidelines for contributing](CONTRIBUTING.md).

## License

Assets in this repository are licensed with the MIT License. For more information, please see [LICENSE.md](LICENSE.md).

## Open-source licenses

This repository and its components use the following open-source projects:

- [Invenietis/CK-ControlChannel-Tcp](https://github.com/Invenietis/CK-ControlChannel-Tcp), licensed under the [MIT License](https://github.com/Invenietis/CK-ControlChannel-Tcp/blob/master/LICENSE.md)
- [Invenietis/CK-ControlChannel-Abstractions](https://github.com/Invenietis/CK-ControlChannel-Abstractions), licensed under the [MIT License](https://github.com/Invenietis/CK-ControlChannel-Abstractions/blob/master/LICENSE.md)
- [Invenietis/CK-Core](https://github.com/Invenietis/CK-Core), licensed under the [GNU Lesser General Public License v3.0](https://github.com/Invenietis/CK-Core/blob/master/LICENSE)
- [Invenietis/CK-Text](https://github.com/Invenietis/CK-Text), licensed under the [MIT License](https://github.com/Invenietis/CK-Text/blob/master/LICENSE)
- [Invenietis/CK-ActivityMonitor](https://github.com/Invenietis/CK-ActivityMonitor), licensed under the [GNU Lesser General Public License v3.0](https://github.com/Invenietis/CK-ActivityMonitor/blob/master/LICENSE)
- [Invenietis/CK-Monitoring](https://github.com/Invenietis/CK-Monitoring), licensed under the [MIT License](https://github.com/Invenietis/CK-Monitoring/blob/master/LICENSE)
