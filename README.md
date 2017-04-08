## Office Add-in Dev Server

IIS Express replacement for debugging [Office Add-ins](https://dev.office.com/docs/add-ins/overview/office-add-ins) on macOS - a .NET Core app which hosts static files using HTTPS

### Build and run instructions:

- Install [.NET Core for macOS](https://www.microsoft.com/net/core#macos)
- Execute `dotnet restore src`
- Execute `dotnet publish src -c release`
- Copy default certificate `etc/certificate/certificate.pfx` to publish directory
- Execute `dotnet oads.dll --server-root <addin_directory>` in publish directory

### Available parameters:

CLI | `settings.json` | Mandatory | Default Value | Purpose
--- | --- | :---: | --- | ---
`--server-port` | `server-port` | No | `44300` | Server port
`--server-root` | `server-root` | Yes | | Server root directory
`--x509-file` | `x509-file` | No | `certificate.pfx` | X.509 certificate file path
`--x509-password` | `x509-password` | No | Empty String | X.509 certificate password

[Tweet this repository](https://twitter.com/intent/tweet?text=Check%20out%20Office%20Add-in%20Dev%20Server%20on%20GitHub&url=https%3A%2F%2Fgithub.com%2Falexanderkozlenko%2Foads)