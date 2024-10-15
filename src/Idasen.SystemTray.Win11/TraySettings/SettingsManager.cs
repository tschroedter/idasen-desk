using System.IO ;
using Autofac ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Serilog ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class SettingsManager (
    ICommonApplicationData commonApplicationData ,
    ISettingsStorage       settingsStorage )
    : ISettingsManager
{
    // todo inject ILogger or use _container
    private ILogger ? _logger;

    private Settings _current = new( ) ;

    public ISettings CurrentSettings => _current ;

    public string SettingsFileName { get ; } = commonApplicationData.ToFullPath ( Constants.SettingsFileName ) ;

    public async Task SaveAsync ( )
    {
        await settingsStorage.SaveSettingsAsync ( SettingsFileName ,
                                                  _current ) ;
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

    public void Initialize ( IContainer container)
    {
        _logger = container.Resolve<ILogger>();

        _logger?.Debug($"{nameof(SettingsManager)} initializing...");
    }

    private async Task AddMissingSettingsNotificationsEnabled ( )
    {
        _logger?.Debug ( $"Add missing setting "                                       +
                        $"{nameof ( Settings.DeviceSettings.NotificationsEnabled )} " +
                        $"to current settings from '{SettingsFileName}'" ) ;

        await LoadAsync ( ).ConfigureAwait ( false ) ;

        _current.DeviceSettings.NotificationsEnabled = Constants.NotificationsEnabled ;

        await SaveAsync ( ) ;
    }
}