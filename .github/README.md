# Microsoft Office Add-in Development Server

A tool for hosting static [Microsoft Office Add-ins](https://docs.microsoft.com/en-us/office/dev/add-ins/overview/office-add-ins) files during development. 

## Project Details

```
dotnet oads.dll [--server-root <value>] [--server-port <value>] [--log-file <value>]
```

| Parameter | Default Value | Purpose |
| --- | --- | --- |
| `--server-root` | `./`| The server root directory |
| `--server-port` | `44300` | The server port |
| `--log-file` | | The log file path |

Run parameters can be provided via via the `oads.json` file located in the add-in or current directory. 

```json
{
    "server-root": "/Users/user/src/",
    "server-port": 44300,
    "log-file": "/Users/user/src/oads.log"
}
```

The server uses an HTTPS certificate named `oads.pfx` without password located in the local application data directory (a new one will be generated if it does not exist).

- Only `GET` requests are supported.
- Connection keep-alive timeout is `60` minutes.
- Configuration file in the current directory has higher priority.

## Usage Examples

```
dotnet oads.dll --server-root /Users/user/src/
```
```
Microsoft Office Add-in Development Server version 1.0.0

Server root: "/Users/user/src/"
Server address: https://localhost:44300

2018-01-02/03:04:05.06+00:00 ERR 404 GET /favicon.ico
2018-01-02/03:04:05.06+00:00 INF 200 GET /app.html
```