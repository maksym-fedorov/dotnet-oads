## Office Add-in Debug Server

Local web server for debugging [Microsoft Office Add-ins](https://dev.office.com/docs/add-ins/overview/office-add-ins) on macOS. The server is based on [.NET Core 2.0](https://www.microsoft.com/net/download/macos) and provides access to static add-in files. Run parameters can be provided via command-line interface or via `settings.json` file located in the same directory with the server assembly.

### Supported parameters:

CLI | File | Mandatory | Default Value | Purpose
--- | --- | :---: | --- | ---
`--server-root` | `server-root` | Yes | | Server root directory
`--server-port` | `server-port` | No | `44300` | Server port
`--x509-file` | `x509-file` | No | `certificate.pfx` | X.509 certificate file path
`--x509-password` | `x509-password` | No | | X.509 certificate password
`--log-file` | `log-file` | No | | Log file path

### Run instruction (CLI):

```
dotnet oads.dll --server-root <value> [--server-port <value>] [--x509-file <value>] [--x509-password <value>] [--log-file]
```

### Settings file structure:

```json
{
    "server-root": "<value>",
    "server-port": <value>,
    "x509-file": "<value>",
    "x509-password": "<value>",
    "log-file": "<value>"
}
```

### Specifics

- The certificate which is going to be used for running an add-in server must be added to the list of trusted OS certificates.

### Limitations

- Only `GET` requests are supported.

### Output sample:

```
Office Add-in Debug Server 1.0.0

Server root: "/Users/user/addin/"
Server port: 44300
X.509 file: "/Users/user/oads/certificate.pfx"
X.509 info: "OU=OFFICE_ADDIN_DEV_SERVER, CN=localhost" (01/01/2018 01:01:01 - 01/01/2020 01:01:01)

2018-01-02T03:04:05.0123456+01:00 404 GET "/favicon.ico"
2018-01-02T03:04:05.0123457+01:00 200 GET "/index.html"
```

[![Latest release](https://img.shields.io/github/release/alexanderkozlenko/office-addin-server.svg?style=flat-square)](https://github.com/alexanderkozlenko/office-addin-server/releases)