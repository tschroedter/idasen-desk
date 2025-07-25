using System.IO ;
using System.Reactive.Subjects ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Serilog ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class SettingsManager ( ILogger                logger ,
                               ICommonApplicationData commonApplicationData ,
                               ISettingsStorage       settingsStorage )
    : ISettingsManager
{
    private readonly ISubject < ISettings > _settingsSaved = new Subject < ISettings > ( ) ;

    private Settings _current = new ( ) ;

    public IObservable < ISettings > SettingsSaved => _settingsSaved ;

    public ISettings CurrentSettings => _current ;

    public string SettingsFileName { get ; } = commonApplicationData.ToFullPath ( Constants.SettingsFileName ) ;

    public async Task SaveAsync ( CancellationToken token )
    {
        await settingsStorage.SaveSettingsAsync ( SettingsFileName ,
                                                  _current ,
                                                  token ) ;

        _settingsSaved.OnNext ( _current ) ;
    }

    public async Task < bool > UpgradeSettingsAsync ( CancellationToken token )
    {
        if ( ! File.Exists ( SettingsFileName ) )
            return true ;

        var settings = await File.ReadAllTextAsync ( SettingsFileName ,
                                                     token )
                                 .ConfigureAwait ( false ) ;

        if ( ! settings.Contains ( nameof ( Settings.DeviceSettings.NotificationsEnabled ) ) )
        {
            await AddMissingSettingsNotificationsEnabled ( token ) ;
        }

        return false ;
    }

    public async Task SetLastKnownDeskHeight ( uint heightInCm , CancellationToken token )
    {
        CurrentSettings.HeightSettings.LastKnownDeskHeight = heightInCm ;

        await SaveAsync ( token ) ;
    }

    public async Task LoadAsync ( CancellationToken token )
    {
        _current = await settingsStorage.LoadSettingsAsync ( SettingsFileName, token ) ;
    }

    private async Task AddMissingSettingsNotificationsEnabled ( CancellationToken token )
    {
        logger.Debug ( $"Add missing setting "                                       +
                       $"{nameof ( Settings.DeviceSettings.NotificationsEnabled )} " +
                       $"to current settings from '{SettingsFileName}'" ) ;

        await LoadAsync ( token ).ConfigureAwait ( false ) ;

        _current.DeviceSettings.NotificationsEnabled = Constants.NotificationsEnabled ;

        await SaveAsync ( token ) ;
    }
}