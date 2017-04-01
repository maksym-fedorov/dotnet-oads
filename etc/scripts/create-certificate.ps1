Param (
    [Parameter(Mandatory=$true)][string]$openSSLPath
)

[string]$outputPath = "$PSScriptRoot\..\certificate"

Set-Item -Path "env:OPENSSL_CONF" -Value "$PSScriptRoot\openssl.cnf" -Force
Set-Item -Path "env:RANDFILE" -Value "${env:TEMP}\openssl.rnd" -Force

& "$openSSLPath\openssl.exe" req -newkey rsa:2048 -nodes -x509 -batch -keyout "$outputPath\certificate.key" -days 1825 -out "$outputPath\certificate.cer" -subj "/CN=localhost/OU=OFFICE_ADDIN_DEV_SERVER"
& "$openSSLPath\openssl.exe" pkcs12 -export -inkey "$outputPath\certificate.key" -in "$outputPath\certificate.cer" -out "$outputPath\certificate.pfx" -passout pass: