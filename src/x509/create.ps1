param (
    [parameter(mandatory)][String]$openSSLPath
)

& "$openSSLPath/openssl" req -config "$PSScriptRoot/openssl.cnf" -new -out "$PSScriptRoot/https.pem" -keyout "$PSScriptRoot/https.key";
& "$openSSLPath/openssl" x509 -req -days 1825 -extfile "$PSScriptRoot/openssl.cnf" -extensions "v3_req" -in "$PSScriptRoot/https.pem" -signkey "$PSScriptRoot/https.key" -out "$PSScriptRoot/https.crt";
& "$openSSLPath/openssl" pkcs12 -export -out "$PSScriptRoot/https.pfx" -inkey "$PSScriptRoot/https.key" -in "$PSScriptRoot/https.crt" -passout pass:"";

remove-item -path "$PSScriptRoot/https.pem" -force;
remove-item -path "$PSScriptRoot/https.crt" -force;