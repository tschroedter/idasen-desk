namespace Idasen.SystemTray.Win11.Settings
{
    public interface ISettingsManager
    {
        ISettings     CurrentSettings  { get ; }
        string        SettingsFileName { get ; }
        Task          Save ( ) ;
        Task          Load ( ) ;
        Task < bool > UpgradeSettings ( ) ;
    }
}