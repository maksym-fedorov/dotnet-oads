## Office Add-in Development Server

IIS Express replacement for debugging [Office Add-ins](https://dev.office.com/docs/add-ins/overview/office-add-ins) on macOS - a .NET Core app which hosts static files using HTTPS

### Run instructions:

- Install [.NET Core for macOS](https://www.microsoft.com/net/core#macos)
- Execute the following commands:

```
dotnet restore ./src
dotnet build ./src -c Release
dotnet ./bin/bin/Release/netcoreapp1.1/oads.dll --server-root <addin-directory>
```

### Available parameters:

CLI | `settings.json` | Mandatory | Default Value | Purpose
--- | --- | :---: | --- | ---
`--server-port` | `server-port` | No | `44300` | Server port
`--server-root` | `server-root` | Yes | | Server root directory
`--x509-file` | `x509-file` | No | `certificate.pfx` | X.509 certificate file path
`--x509-password` | `x509-password` | No | Empty String | X.509 certificate password

[![Latest release](https://img.shields.io/github/release/alexanderkozlenko/office-addin-server.svg)](https://github.com/alexanderkozlenko/office-addin-server/releases) [![Tweet this repository](https://img.shields.io/twitter/url/http/shields.io.svg?style=social)](https://twitter.com/intent/tweet?text=Check%20out%20Office%20Add-in%20Dev%20Server%20on%20GitHub&url=https%3A%2F%2Fgithub.com%2Falexanderkozlenko%2Foffice-addin-server)