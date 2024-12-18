using Idasen.SystemTray.Win11.TraySettings ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface ISettingsStorage
{
    Task < Settings > LoadSettingsAsync ( string settingsFileName ) ;
    Task              SaveSettingsAsync ( string settingsFileName , Settings settings ) ;
}