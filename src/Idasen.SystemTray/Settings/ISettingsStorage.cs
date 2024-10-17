using System.Threading.Tasks ;

namespace Idasen.SystemTray.Settings
{
    public interface ISettingsStorage
    {
        Task < Settings > LoadSettingsAsync ( string settingsFileName ) ;
        Task              SaveSettingsAsync ( string settingsFileName , Settings settings ) ;
    }
}