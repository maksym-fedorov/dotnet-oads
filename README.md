## Office Add-in Development Server

IIS Express replacement for debugging [Office Add-ins](https://dev.office.com/docs/add-ins/overview/office-add-ins) on macOS - a .NET Core app which hosts static files using HTTPS

### Run instructions:

- Install [.NET Core for macOS](https://www.microsoft.com/net/download/macos)
- Execute the following commands:

```
dotnet build ./src -c Release
dotnet ./bin/bin/oads/Release/netcoreapp2.0/oads.dll --server-root <addin-directory>
```

### Available parameters:

CLI | `settings.json` | Mandatory | Default Value | Purpose
--- | --- | :---: | --- | ---
`--server-root` | `server-root` | Yes | | Server root directory
`--server-port` | `server-port` | No | `44300` | Server port
`--x509-file` | `x509-file` | No | `certificate.pfx` | X.509 certificate file path
`--x509-password` | `x509-password` | No | Empty String | X.509 certificate password

[![Latest release](https://img.shields.io/github/release/alexanderkozlenko/office-addin-server.svg?style=flat-square)](https://github.com/alexanderkozlenko/office-addin-server/releases)