## Office Add-in Dev Server

IIS Express replacement for debugging [Office Add-ins](https://dev.office.com/docs/add-ins/overview/office-add-ins) on macOS - a .NET Core app which can host static files via HTTPS.

### Build instructions:

1. Install [.NET Core for macOS](https://www.microsoft.com/net/core#macos)
2. `dotnet restore src`
3. `dotnet build src`
4. `dotnet publish src -c Release`

Output files are located at `bin/bin/Release/netcoreapp1.1/publish/`.

### Run instructions:

Server supports the following options:

Short Form | Long Form | Mandatory | Default Value | Purpose
--- | --- | :---: | --- | ---
`-sp` | `--server-port` | No | `44300` | Server port
`-sr` | `--server-root` | Yes | | Server root directory
`-cf` | `--cert-file` | No | `./certificate.pfx` | Certificate file in PKCS #12 format
`-cp` | `--cert-password` | No | Empty String | Certificate password

Repository contains a self-signed certificate located at `etc/certificate/certificate.pfx` without password, which can be used as a default (however it should be added to a keychain at first).

Server command line with the default options is:

`dotnet oads.dll -sr /office_addin_files/`