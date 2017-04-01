## Office Add-in Dev Server

IIS Express replacement for debugging [Office Add-ins](https://dev.office.com/docs/add-ins/overview/office-add-ins) on macOS - a .NET Core app which can host static files via HTTPS.

### Build instructions:

Run the following commands:

1. `dotnet restore src`
2. `dotnet build src`
3. `dotnet publish src -c Release`

Output files are located at `bin/bin/Release/netcoreapp1.1/publish/`.

### Run instructions:

Server requires an SSL certificate in PKCS #12 format named `certificate.pfx` in the same directory. Repository contains a self-signed certificate located at `etc/certificate/certificate.pfx` with the password `OFFICE_ADDIN_DEV_SERVER`, which can be used as a default (however it should be added to a keychain at first).

Server supports the following options:

Short Form | Long Form | Mandatory | Purpose
--- | --- | :---: | ---
`-sp` | `--server-port` | `Yes` | Server port
`-sr` | `--site-root` | `Yes` | Office add-in files directory
`-cp` | `--cert-password` | `No` | Certificate password

Server command line with the default options is:

`dotnet bin/bin/Release/netcoreapp1.1/publish/oads.dll --server-port 44300 --site-root /office_addin_files/ --cert-password OFFICE_ADDIN_DEV_SERVER`