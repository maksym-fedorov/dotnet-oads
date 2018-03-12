## Office Add-in Debug Server

Local web server for debugging [Microsoft Office Add-ins](https://docs.microsoft.com/en-us/office/dev/add-ins/overview/office-add-ins) on macOS. The server is based on [.NET Core 2.0](https://www.microsoft.com/net/download/macos) and provides access to static add-in files.

```
dotnet oads.dll [--server-root <value>] [--server-port <value>] [--log-file <value>]
```

Parameter | Default Value | Purpose
--- | --- | ---
`--server-root` | `./`| The server root directory
`--server-port` | `44300` | The server port
`--log-file` | | The log file path

Run parameters can be provided via via the `oads.json` file located in the application or current directory. 

```json
{
    "server-root": "<value>",
    "server-port": <value>,
    "log-file": "<value>"
}
```

Parameter | Default Value | Purpose
--- | --- | ---
`server-root` | `./`| The server root directory
`server-port` | `44300` | The server port
`log-file` | | The log file path

The server requires an HTTPS certificate named `https.pfx` without password located in the application directory. Such test certificate can be created via the `oads-cert` tool.

```
dotnet oads-cert.dll --command <value> [--cert-file <value>]
```

Parameter | Default Value | Purpose
--- | --- | ---
`--command` | | The command to execute
`--cert-file` | `./https.pfx` | The certificate file path

Command | Purpose
--- | ---
`create` | Create new self-signed test certificate

### Specifics

- The server certificate must be added to the list of trusted OS certificates.
- Configuration file in the current directory has higher priority.
- Connection keep-alive timeout is `60` minutes.
- Only `GET` requests are supported.
- A generated test certificate is valid for `1` year.

### Examples

```
dotnet oads-cert.dll --command create
```
```
Office Add-in Debug Certificate Manager version 1.0.0

Created a certificate at "/Users/user/oads/https.pfx"

Certificate period: 01/02/2018 - 01/02/2019 (UTC)
Certificate thumbprint: 4034E13C94B5A3C4006F1C2EBC901488ADF438E8
```
```
dotnet oads.dll --server-root /Users/user/src/
```
```
Office Add-in Debug Server version 1.0.0

Server root: "/Users/user/src/"
Server address: https://localhost:44300

2018-01-02/03:04:05.06+00:00 ERR 404 GET /favicon.ico
2018-01-02/03:04:05.06+00:00 INF 200 GET /app.html
```

[![Latest release](https://img.shields.io/github/release/alexanderkozlenko/office-addin-server.svg?style=flat-square)](https://github.com/alexanderkozlenko/office-addin-server/releases)