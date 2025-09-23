namespace Idasen.SystemTray.Win11.Interfaces ;

public interface ISettingsManager
{
    ISettings                 CurrentSettings  { get ; }
    string                    SettingsFileName { get ; }
    IObservable < ISettings > SettingsSaved    { get ; }
    Task                      SaveAsync ( CancellationToken            token ) ;
    Task                      LoadAsync ( CancellationToken            token ) ;
    Task < bool >             UpgradeSettingsAsync ( CancellationToken token ) ;
    Task                      SetLastKnownDeskHeight ( uint            heightInCm , CancellationToken token ) ;
    Task                      ResetSettingsAsync ( CancellationToken   token ) ;
}