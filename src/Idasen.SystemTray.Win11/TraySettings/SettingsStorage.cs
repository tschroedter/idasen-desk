using System.Diagnostics.CodeAnalysis ;
using System.IO ;
using System.IO.Abstractions ;
using System.Text.Json ;
using System.Text.Json.Serialization ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class SettingsStorage ( IFileSystem fileSystem ) : ISettingsStorage
{
    // Win32 HResults for sharing / lock violations.
    private const int SharingViolationHResult = unchecked ( ( int )0x80070020 ) ; // ERROR_SHARING_VIOLATION
    private const int LockViolationHResult    = unchecked ( ( int )0x80070021 ) ; // ERROR_LOCK_VIOLATION

    private const int MaxSaveAttempts  = 5 ;
    private const int InitialBackoffMs = 50 ;

    public static readonly JsonSerializerOptions JsonOptions = new( )
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull ,
        PropertyNamingPolicy   = JsonNamingPolicy.CamelCase
    } ;

    // In-process coordination (cheap). Prevents overlapping Save/Load inside this process.
    private static readonly SemaphoreSlim IoLock = new(1 ,
                                                       1) ;

    public async Task < Settings > LoadSettingsAsync ( string            settingsFileName ,
                                                       CancellationToken token )
    {
        await IoLock.WaitAsync ( token ).ConfigureAwait ( false ) ;

        try
        {
            try
            {
                if ( ! fileSystem.File.Exists ( settingsFileName ) )
                {
                    // Create defaults and pick the current theme from Windows if available
                    var defaults = new Settings
                    {
                        AppearanceSettings =
                        {
                            ThemeName = ThemeDefaults.GetDefaultThemeName ( )
                        }
                    } ;
                    return defaults ;
                }

                await using var openStream = fileSystem.File.Open ( settingsFileName ,
                                                                    FileMode.Open ,
                                                                    FileAccess.Read ,
                                                                    FileShare.Read ) ;
                var result = await JsonSerializer.DeserializeAsync < Settings > ( openStream ,
                                 JsonOptions ,
                                 token ).ConfigureAwait ( false ) ;

                return result ?? new Settings ( ) ;
            }
            catch ( Exception ex )
            {
                throw new IOException ( $"Failed to load settings from {settingsFileName}" ,
                                        ex ) ;
            }
        }
        finally
        {
            IoLock.Release ( ) ;
        }
    }

    public async Task SaveSettingsAsync ( string            settingsFileName ,
                                          Settings          settings ,
                                          CancellationToken token )
    {
        await IoLock.WaitAsync ( token ).ConfigureAwait ( false ) ;

        try
        {
            var directoryName = fileSystem.Path.GetDirectoryName ( settingsFileName ) ;

            if ( string.IsNullOrEmpty ( directoryName ) )
                throw new IOException ( $"Failed to get directory name from {settingsFileName}" ) ;

            if ( ! fileSystem.Directory.Exists ( directoryName ) )
                fileSystem.Directory.CreateDirectory ( directoryName ) ;

            // Serialize first in-memory to keep on-disk lock time minimal
            await using var ms = new MemoryStream ( ) ;
            await JsonSerializer.SerializeAsync ( ms ,
                                                  settings ,
                                                  JsonOptions ,
                                                  token ).ConfigureAwait ( false ) ;
            ms.Position = 0 ;

            var tempFile = settingsFileName + ".tmp" ;

            for ( var attempt = 1 ; attempt <= MaxSaveAttempts ; attempt ++ )
            {
                token.ThrowIfCancellationRequested ( ) ;

                try
                {
                    // Write temp file (exclusive). Use Create -> overwrite if exists.
                    await using ( var fs = fileSystem.File.Open ( tempFile ,
                                                                  FileMode.Create ,
                                                                  FileAccess.Write ,
                                                                  FileShare.None ) )
                    {
                        ms.Position = 0 ;
                        await ms.CopyToAsync ( fs ,
                                               token ).ConfigureAwait ( false ) ;
                        await fs.FlushAsync ( token ).ConfigureAwait ( false ) ;
                    }

                    // Atomic replace if target exists, otherwise move.
                    if ( fileSystem.File.Exists ( settingsFileName ) )
                        // Prefer atomic replace if available
                        fileSystem.File.Replace ( tempFile ,
                                                  settingsFileName ,
                                                  null ) ;
                    else
                        fileSystem.File.Move ( tempFile ,
                                               settingsFileName ) ;

                    return ; // Success
                }
                catch ( IOException ioEx ) when ( IsSharingOrLockViolation ( ioEx ) && attempt < MaxSaveAttempts )
                {
                    // Backoff with simple exponential delay
                    var delay = InitialBackoffMs * ( 1 << ( attempt - 1 ) ) ;

                    try
                    {
                        await Task.Delay ( delay ,
                                           token ).ConfigureAwait ( false ) ;
                    }
                    finally
                    {
                        // Clean temp between retries if it lingered
                        SafeDelete ( tempFile ) ;
                    }
                }
                catch ( Exception ex )
                {
                    // Clean temp and rethrow wrapped
                    SafeDelete ( tempFile ) ;
                    throw new IOException ( $"Failed to save settings to {settingsFileName}" ,
                                            ex ) ;
                }
            }

            // If all retries failed we throw a final explicit error
            throw new
                IOException ( $"Failed to save settings to {settingsFileName} after {MaxSaveAttempts} attempts due to sharing/lock violations." ) ;
        }
        finally
        {
            IoLock.Release ( ) ;
        }
    }

    [ ExcludeFromCodeCoverage ]
    private static bool IsSharingOrLockViolation ( IOException ex )
    {
        return ex.HResult is SharingViolationHResult or LockViolationHResult ;
    }

    [ ExcludeFromCodeCoverage ]
    private void SafeDelete ( string path )
    {
        try
        {
            if ( fileSystem.File.Exists ( path ) )
                fileSystem.File.Delete ( path ) ;
        }
        catch
        {
            // Swallow best effort cleanup
        }
    }
}