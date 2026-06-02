using Idasen.SystemTray.Win11.TraySettings ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface ISettingsStorage
{
    ValueTask < Settings > LoadSettingsAsync ( string            settingsFileName ,
                                               CancellationToken token ) ;

    ValueTask SaveSettingsAsync ( string            settingsFileName ,
                                  Settings          settings ,
                                  CancellationToken token ) ;
}