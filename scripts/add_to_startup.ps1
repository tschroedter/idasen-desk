$x86 = "C:\Temp\" #[Environment]::GetEnvironmentVariable("ProgramFiles(x86)")
$pwd = Get-Location
$pwdallfiles = Join-Path -Path $pwd -ChildPath "*.*"
$startupFolder = [Environment]::GetFolderPath('Startup')

$appname = "Idasen"
$appexe = $appname + ".SystemTray.exe"
$applnk = $appexe + ".lnk"

$installpath = Join-Path -Path $x86 -ChildPath $appname
$fullpathexe = Join-Path -Path $x86 -ChildPath $appexe
$fullpathlnk = Join-Path -Path $startupFolder -ChildPath $applnk

# create installation folder
if (-not (Test-Path -LiteralPath $installpath)) {
    
    try {
        New-Item -Path $installpath -ItemType Directory -Force -ErrorAction Stop | Out-Null
    }
    catch {
        Write-Error -Message "Unable to create directory '$installpath'. Error was: $_" -ErrorAction Stop
    }
    "Successfully created directory '$installpath'."

}
else {
    "Directory already existed"
}

# copy files
Write-Host "Copying all files from "$pwdallfiles" into folder "$installpath

Copy-Item -Path $pwdallfiles -Destination $installpath 

# create shortcut in Startup folder
Write-Host "Creating a shortcut for "$appname" in your startup folder..."

$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut($fullpathlnk)
$shortcut.TargetPath = $fullpathexe
$shortcut.IconLocation = $fullpathexe
$shortcut.Save()

Write-Host "Done"