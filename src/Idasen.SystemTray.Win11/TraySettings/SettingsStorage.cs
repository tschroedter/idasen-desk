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
        await using var openStream = File.OpenRead ( settingsFileName ) ;
        var result = await JsonSerializer.DeserializeAsync < Settings > ( openStream ,
                                                                          _jsonOptions ) ??
                     new Settings ( ) ;

        return result ;
    }

    public async Task SaveSettingsAsync ( string   settingsFileName ,
                                          Settings settings )
    {
        await using var stream = File.Create ( settingsFileName ) ;
        await JsonSerializer.SerializeAsync ( stream , settings , _jsonOptions ) ;
    }
}