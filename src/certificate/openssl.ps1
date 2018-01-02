param (
    [parameter(mandatory)][String]$openSSLPath
)

function execute {
    param(
        [parameter(mandatory)][ScriptBlock]$command
    )

    & $command; 
    
    if ($lastexitcode -eq $null) {
        throw "`"$command`" command failed";
    }
    if ($lastexitcode -ne 0) {
        throw "`"$command`" command failed with code $lastexitcode";
    }
}

[String]$tempPath = [IO.Path]::GetTempPath();
[String]$name = "certificate";

set-item -force -path "env:OPENSSL_CONF" -value "$PSScriptRoot/openssl.cnf";
set-item -force -path "env:RANDFILE" -value "$tempPath/openssl.rnd";

execute { "$openSSLPath/openssl" req -newkey rsa:2048 -nodes -x509 -batch -keyout "$PSScriptRoot/$name.key" -days 1825 -out "$PSScriptRoot/$name.cer" -subj "/CN=localhost/OU=OFFICE_ADDIN_DEV_SERVER" };
execute { "$openSSLPath/openssl" pkcs12 -export -inkey "$PSScriptRoot/$name.key" -in "$PSScriptRoot/$name.cer" -out "$PSScriptRoot/$name.pfx" -passout pass: };