using Autofac ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface ISettingsManager
{
    ISettings     CurrentSettings  { get ; }
    string        SettingsFileName { get ; }
    Task          SaveAsync ( ) ;
    Task          LoadAsync ( ) ;
    Task < bool > UpgradeSettingsAsync ( ) ;
    void Initialize ( IContainer container) ;
}