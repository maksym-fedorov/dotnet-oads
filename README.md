## Office Add-in Dev Server

IIS Express replacement for debugging [Office Add-ins](https://dev.office.com/docs/add-ins/overview/office-add-ins) on macOS - a .NET Core app which hosts static files using HTTPS.

### Build and run instructions:

1. Install [.NET Core for macOS](https://www.microsoft.com/net/core#macos)
2. Execute `dotnet restore src`
3. Execute `dotnet build src`
4. Execute `dotnet publish src -c Release`
5. Copy default certificate `etc/certificate/certificate.pfx` to `bin/bin/Release/netcoreapp1.1/publish/`
6. Execute `dotnet bin/bin/Release/netcoreapp1.1/publish/oads.dll -sr <addin_directory>`

### Available options:

Option | Short Form | Mandatory | Default Value | Purpose
--- | --- | :---: | --- | ---
`--server-port` | `-sp` | No | `44300` | Server port
`--server-root` | `-sr` | Yes | | Server root directory
`--cert-file` | `-cf` | No | `./certificate.pfx` | Certificate file in PKCS #12 format
`--cert-password` | `-cp` | No | Empty String | Certificate password