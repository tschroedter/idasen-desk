using System.IO ;
using System.IO.Abstractions ;
using System.Text.Json ;
using System.Text.Json.Serialization ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.TraySettings;

public class SettingsStorage ( IFileSystem fileSystem ) : ISettingsStorage
{
    public static readonly JsonSerializerOptions JsonOptions = new ( )
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull ,
        PropertyNamingPolicy   = JsonNamingPolicy.CamelCase
    } ;

    public async Task < Settings > LoadSettingsAsync ( string            settingsFileName ,
                                                       CancellationToken token )
    {
        try
        {
            if ( ! fileSystem.File.Exists ( settingsFileName ) )
                return new Settings ( ) ;

            await using var openStream = fileSystem.File.OpenRead ( settingsFileName ) ;
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

    public async Task SaveSettingsAsync ( string            settingsFileName ,
                                          Settings          settings ,
                                          CancellationToken token )
    {
        try
        {
            var directoryName = Path.GetDirectoryName ( settingsFileName ) ;

            if ( string.IsNullOrEmpty ( directoryName ) )
                throw new IOException ( $"Failed to get directory name from {settingsFileName}" ) ;

            if ( ! Directory.Exists ( directoryName ) )
                Directory.CreateDirectory ( directoryName ) ;

            await using var stream = fileSystem.File.Create ( settingsFileName ) ;

            await JsonSerializer.SerializeAsync ( stream ,
                                                  settings ,
                                                  JsonOptions ,
                                                  token ).ConfigureAwait ( false ) ;
        }
        catch ( Exception ex )
        {
            throw new IOException ( $"Failed to save settings to {settingsFileName}" ,
                                    ex ) ;
        }
    }
}