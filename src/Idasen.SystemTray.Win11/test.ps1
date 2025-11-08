$exePath = "C:\Users\thomas\source\repos\idasen-desk\src\Idasen.SystemTray.Win11\bin\Debug\net8.0-windows10.0.19041\win-x64\Idasen.SystemTray.exe" # adjust
$settings = "C:\ProgramData\Idasen.SystemTray\Settings.json"          # adjust to actual file
$log = "C:\ProgramData\Idasen.SystemTray\logs\Idasen.SystemTray_017.log"                                # adjust

function Snapshot {
    $t = Get-Date -Format o
    $exists = Test-Path $settings
    $len = if ($exists) { (Get-Item $settings).Length } else { -1 }
    $lm  = if ($exists) { (Get-Item $settings).LastWriteTimeUtc } else { "" }
    "$t, Exists=$exists, Length=$len, LastWrite=$lm" | Out-File results.csv -Append
    if (Test-Path $log) {
        Select-String -Path $log -Pattern "Failed to load settings" -SimpleMatch | Out-File results.csv -Append
    }
}

for ($i=1; $i -le 10; $i++) {
    Snapshot
    $p = Start-Process -FilePath $exePath -PassThru
    Start-Sleep -Seconds 4
    $p.CloseMainWindow() | Out-Null
    Start-Sleep -Seconds 2
    if (!$p.HasExited) { $p.Kill() }
    Start-Sleep -Seconds 1
    Snapshot
}