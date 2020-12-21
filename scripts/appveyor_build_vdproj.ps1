pwd

Write-Host $env:APPVEYOR_BUILD_VERSION

$pwd = Get-Location

$vdprojFileName = "Idasen.SystemTray.Setup\Idasen.SystemTray.Setup.vdproj"
$vdproj = Join-Path -Path $pwd -ChildPath $vdprojFileName

$searchProductVersion = '"ProductVersion" = "8:.*"'
$replaceProductVersion = '"ProductVersion" = "8:' + $env:APPVEYOR_BUILD_VERSION + '"'

$content = [IO.File]::ReadAllText($vdproj)


Write-Host $searchProductVersion
Write-Host $replaceProductVersion
$content = $content -Replace $searchProductVersion, $replaceProductVersion

$productCode = [guid]::NewGuid()
$searchProductCode = '"ProductCode" = "8:{.*}"'
$replaceProductCode = '"ProductCode" = "8:{' + $productCode.ToString().ToUpper() + '}"'

Write-Host $productCode
Write-Host $searchProductCode
Write-Host $replaceProductCode
$content = $content -Replace $searchProductCode, $replaceProductCode

Write-Host $content

Set-Content -Path $vdproj -Value $content

appveyor version
cmd /c '"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.com" .\Idasen-Desk.sln /build "Debug|Any CPU" /project .\Idasen.SystemTray.Setup\Idasen.SystemTray.Setup.vdproj'