param(
  [ValidateSet('Debug','Release')][string]$Configuration = 'Release'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Set plain text output rendering only if PSStyle exists (PowerShell 7+)
$psStyleVar = Get-Variable -Name PSStyle -ErrorAction SilentlyContinue
if ($psStyleVar) {
  $PSStyle.OutputRendering = 'PlainText'
}

Push-Location $PSScriptRoot/..
try {
  $appProj  = "src/Idasen.SystemTray.Win11/Idasen.SystemTray.Win11.csproj"
  $testProj = "src/Idasen.SystemTray.Win11.Tests/Idasen.SystemTray.Win11.Tests.csproj"

  Write-Host "restoring..." -ForegroundColor Cyan
  dotnet restore $appProj
  dotnet restore $testProj

  Write-Host "building..." -ForegroundColor Cyan
  dotnet build $appProj  --configuration $Configuration --no-restore
  dotnet build $testProj --configuration $Configuration --no-restore

  Write-Host "testing..." -ForegroundColor Cyan
  dotnet test $testProj --configuration $Configuration --no-build --verbosity normal

  Write-Host "publishing app (win-x64)..." -ForegroundColor Cyan
  dotnet publish $appProj `
    --configuration $Configuration `
    --runtime win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:PublishReadyToRun=true

  # Zip the published executable with version
  $projPath = $appProj
  [xml]$projXml = Get-Content $projPath
  $version = ($projXml.Project.PropertyGroup | ForEach-Object { $_.Version } | Where-Object { $_ } | Select-Object -First 1)
  if (-not $version) { throw "Version not found in $projPath" }

  $publishDir = "src/Idasen.SystemTray.Win11/bin/$Configuration/net8.0-windows10.0.19041/win-x64/publish"
  $exe = Join-Path $publishDir "Idasen.SystemTray.exe"
  if (-not (Test-Path $exe)) { throw "Executable not found: $exe" }

  $zip = Join-Path $publishDir ("Idasen.SystemTray-{0}-win-x64.zip" -f $version)
  if (Test-Path $zip) { Remove-Item $zip -Force }
  Write-Host "zipping $exe -> $zip" -ForegroundColor Cyan
  Compress-Archive -Path $exe -DestinationPath $zip -Force

  Write-Host "done" -ForegroundColor Green
}
finally {
  Pop-Location
}
