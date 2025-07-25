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

    public async Task SaveAsync ( )
    {
        await settingsStorage.SaveSettingsAsync ( SettingsFileName ,
                                                  _current ) ;

        _settingsSaved.OnNext ( _current ) ;
    }

    public async Task LoadAsync ( )
    {
        _current = await settingsStorage.LoadSettingsAsync ( SettingsFileName ) ;
    }

    public async Task < bool > UpgradeSettingsAsync ( )
    {
        if ( ! File.Exists ( SettingsFileName ) )
            return true ;

        var settings = await File.ReadAllTextAsync ( SettingsFileName )
                                 .ConfigureAwait ( false ) ;

        if ( ! settings.Contains ( nameof ( Settings.DeviceSettings.NotificationsEnabled ) ) )
        {
            await AddMissingSettingsNotificationsEnabled ( ) ;
        }

        return false ;
    }

    public async Task SetLastKnownDeskHeight ( uint heightInCm )
    {
        CurrentSettings.HeightSettings.LastKnownDeskHeight = heightInCm ;

        await SaveAsync ( ) ;
    }

    private async Task AddMissingSettingsNotificationsEnabled ( )
    {
        logger.Debug ( $"Add missing setting "                                       +
                       $"{nameof ( Settings.DeviceSettings.NotificationsEnabled )} " +
                       $"to current settings from '{SettingsFileName}'" ) ;

        await LoadAsync ( ).ConfigureAwait ( false ) ;

        _current.DeviceSettings.NotificationsEnabled = Constants.NotificationsEnabled ;

        await SaveAsync ( ) ;
    }
}