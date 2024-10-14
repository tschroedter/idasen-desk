namespace Idasen.SystemTray.Win11.Interfaces ;

public interface ISettingsManager
{
    ISettings     CurrentSettings  { get ; }
    string        SettingsFileName { get ; }
    Task          Save ( ) ;
    Task          Load ( ) ;
    Task < bool > UpgradeSettings ( ) ;
}