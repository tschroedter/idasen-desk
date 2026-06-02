namespace Idasen.SystemTray.Win11.Interfaces ;

public interface ISettingsManager
{
    ISettings                 CurrentSettings  { get ; }
    string                    SettingsFileName { get ; }
    IObservable < ISettings > SettingsSaved    { get ; }
    ValueTask                 SaveAsync ( CancellationToken            token ) ;
    ValueTask                 LoadAsync ( CancellationToken            token ) ;
    ValueTask < bool >        UpgradeSettingsAsync ( CancellationToken token ) ;
    ValueTask                 SetLastKnownDeskHeight ( uint            heightInCm , CancellationToken token ) ;
    ValueTask                 ResetSettingsAsync ( CancellationToken   token ) ;
}