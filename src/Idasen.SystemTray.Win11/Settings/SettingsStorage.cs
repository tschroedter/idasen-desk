using System.IO ;
using System.Text.Json ;
using System.Text.Json.Serialization ;

namespace Idasen.SystemTray.Win11.Settings
{
    public class SettingsStorage : ISettingsStorage
    {
        public async Task < Settings > LoadSettingsAsync ( string settingsFileName )
        {
            await using var openStream = File.OpenRead ( settingsFileName ) ;
            var result = await JsonSerializer.DeserializeAsync < Settings > ( openStream ,
                                                                              _jsonOptions ) ;

            result.DeviceSettings ??= new DeviceSettings ( ) ;
            result.HeightSettings ??= new HeightSettings ( ) ;

            return result ;
        }

        public async Task SaveSettingsAsync ( string   settingsFileName ,
                                              Settings settings )
        {
            await using var stream = File.Create ( settingsFileName ) ;
            await JsonSerializer.SerializeAsync ( stream , settings , _jsonOptions ) ;
        }

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
                                                              {
                                                                  DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull ,
                                                                  PropertyNamingPolicy   = JsonNamingPolicy.CamelCase
                                                              } ;
    }
}