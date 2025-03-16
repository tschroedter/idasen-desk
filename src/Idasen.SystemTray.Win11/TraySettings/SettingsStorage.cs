using System.IO ;
using System.Text.Json ;
using System.Text.Json.Serialization ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class SettingsStorage : ISettingsStorage
{
    private readonly JsonSerializerOptions _jsonOptions = new( )
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull ,
        PropertyNamingPolicy   = JsonNamingPolicy.CamelCase
    } ;

    public async Task < Settings > LoadSettingsAsync ( string settingsFileName )
    {
        try
        {
            if ( ! File.Exists ( settingsFileName ) )
                return new Settings ( ) ;

            await using var openStream = File.OpenRead ( settingsFileName ) ;
            var             result     = await JsonSerializer.DeserializeAsync < Settings > ( openStream ,
                                                                                              _jsonOptions ) ;

            return result ?? new Settings ( ) ;
        }
        catch ( Exception ex )
        {
            throw new IOException ( $"Failed to load settings from {settingsFileName}" ,
                                    ex ) ;
        }
    }

    public async Task SaveSettingsAsync ( string settingsFileName , Settings settings )
    {
        try
        {
            var directoryName = Path.GetDirectoryName ( settingsFileName ) ;

            if (string.IsNullOrEmpty ( directoryName ))
                throw new IOException($"Failed to get directory name from {settingsFileName}");

            if ( !Directory.Exists ( directoryName))
                Directory.CreateDirectory(directoryName);

            await using var stream = File.Create ( settingsFileName ) ;

            await JsonSerializer.SerializeAsync ( stream ,
                                                  settings , 
                                                  _jsonOptions ) ;
        }
        catch ( Exception ex )
        {
            throw new IOException ( $"Failed to save settings to {settingsFileName}" ,
                                    ex ) ;
        }
    }
}