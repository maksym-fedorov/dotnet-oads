## Office Add-in Development Server

Local web server based on [.NET Core](https://www.microsoft.com/net/download/macos) for debugging [Microsoft Office Add-ins](https://dev.office.com/docs/add-ins/overview/office-add-ins) on macOS.

### Run instruction:

```
dotnet oads.dll --server-root <value> [--server-port <value>] [--x509-file <value>] [--x509-password <value>]
```

### Supported parameters:

CLI | `settings.json` | Mandatory | Default Value | Purpose
--- | --- | :---: | --- | ---
`--server-root` | `server-root` | Yes | | Server root directory
`--server-port` | `server-port` | No | `44300` | Server port
`--x509-file` | `x509-file` | No | `certificate.pfx` | X.509 certificate file path
`--x509-password` | `x509-password` | No | | X.509 certificate password

[![Latest release](https://img.shields.io/github/release/alexanderkozlenko/office-addin-server.svg?style=flat-square)](https://github.com/alexanderkozlenko/office-addin-server/releases)