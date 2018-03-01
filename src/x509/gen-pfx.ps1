param (
    [parameter(mandatory)][String]$openSSLPath,
    [parameter(mandatory)][String]$fileName
)

set-item -force -path "env:OPENSSL_CONF" -value "$PSScriptRoot/openssl.cnf";
set-item -force -path "env:RANDFILE" -value "$([IO.Path]::GetTempPath())/openssl.rnd";

& "$openSSLPath/openssl" req -newkey rsa:2048 -nodes -x509 -batch -keyout "$PSScriptRoot/$fileName.key" -days 1825 -out "$PSScriptRoot/$fileName.cer" -subj "/CN=localhost/OU=DO_NOT_TRUST_OFFICE_ADDIN_DEBUG";
& "$openSSLPath/openssl" pkcs12 -export -inkey "$PSScriptRoot/$fileName.key" -in "$PSScriptRoot/$fileName.cer" -out "$PSScriptRoot/$fileName.pfx" -passout pass:"";

remove-item -path "$PSScriptRoot/$fileName.cer" -force;