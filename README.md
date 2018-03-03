## Office Add-in Debug Server

Local web server for debugging [Microsoft Office Add-ins](https://dev.office.com/docs/add-ins/overview/office-add-ins) on macOS. The server is based on [.NET Core 2.0](https://www.microsoft.com/net/download/macos) and provides access to static add-in files. Run parameters can be provided via command-line interface or via the `oads.json` file located in the application directory.

CLI | File | Mandatory | Default Value | Purpose
--- | --- | :---: | --- | ---
`--server-root` | `server-root` | Yes | | The server root directory
`--server-port` | `server-port` | No | `44300` | The server port
`--x509-file` | `x509-file` | No | `oads.pfx` | The server certificate file path
`--x509-pass` | `x509-pass` | No | | The server certificate password
`--log-file` | `log-file` | No | | The log file path

```
dotnet oads.dll \
    --server-root <value> [--server-port <value>] \
    [--x509-file <value>] [--x509-pass <value>] \
    [--log-file <value>]
```
```json
{
    "server-root": "<value>",
    "server-port": <value>,
    "x509-file": "<value>",
    "x509-pass": "<value>",
    "log-file": "<value>"
}
```

### Specifics

- The server certificate must be added to the list of trusted OS certificates.
- Connection keep-alive timeout is 60 minutes.

### Limitations

- Only `GET` requests are supported.

### Examples

```
Office Add-in Debug Server version 1.0.0

Listening https://localhost:44300/ for incoming connections...

2018-01-02/03:04:05.06+00:00 WARN 404 GET "/favicon.ico"
2018-01-02/03:04:05.06+00:00 INFO 200 GET "/app.html"
```

[![Latest release](https://img.shields.io/github/release/alexanderkozlenko/office-addin-server.svg?style=flat-square)](https://github.com/alexanderkozlenko/office-addin-server/releases)