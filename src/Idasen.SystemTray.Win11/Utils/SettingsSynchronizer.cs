using Idasen.SystemTray.Win11.Interfaces;
using Serilog;
using Wpf.Ui.Appearance;

namespace Idasen.SystemTray.Win11.Utils ;

public class SettingsSynchronizer ( ILogger                        logger ,
                                    ILoggingSettingsManager        settingsManager ,
                                    IDoubleToUIntConverter         toUIntConverter ,
                                    IDeviceNameConverter           nameConverter ,
                                    IDeviceAddressToULongConverter addressConverter ,
                                    INotifySettingsChanges         settingsChanges ,
                                    IThemeSwitcher                 themeSwitcher ) : ISettingsSynchronizer
{
    public async Task LoadSettingsAsync ( ISettingsViewModel model ,
                                          CancellationToken  token )
    {
        if ( model == null ) throw new ArgumentNullException ( nameof ( model ) ) ;

        try
        {
            logger.Debug ( "LoadSettingsAsync settings" ) ;

            await settingsManager.LoadAsync ( token ).ConfigureAwait ( false ) ;

            var current = settingsManager.CurrentSettings ;

            model.MinHeight = current.HeightSettings.DeskMinHeightInCm ;
            model.MaxHeight = current.HeightSettings.DeskMaxHeightInCm ;
            model.Standing  = current.HeightSettings.StandingHeightInCm ;
            model.Seating   = current.HeightSettings.SeatingHeightInCm ;
            model.Custom1   = current.HeightSettings.Custom1HeightInCm ;
            model.Custom2   = current.HeightSettings.Custom2HeightInCm ;

            model.StandingIsVisibleInContextMenu = current.HeightSettings.StandingIsVisibleInContextMenu ;
            model.SeatingIsVisibleInContextMenu  = current.HeightSettings.SeatingIsVisibleInContextMenu ;
            model.Custom1IsVisibleInContextMenu  = current.HeightSettings.Custom1IsVisibleInContextMenu ;
            model.Custom2IsVisibleInContextMenu  = current.HeightSettings.Custom2IsVisibleInContextMenu ;
            model.StopIsVisibleInContextMenu     = current.DeviceSettings.StopIsVisibleInContextMenu ;

            model.LastKnownDeskHeight = current.HeightSettings.LastKnownDeskHeight ;
            model.DeskName            = nameConverter.EmptyIfDefault ( current.DeviceSettings.DeviceName ) ;
            model.DeskAddress         = addressConverter.EmptyIfDefault ( current.DeviceSettings.DeviceAddress ) ;
            model.ParentalLock        = current.DeviceSettings.DeviceLocked ;
            model.Notifications       = current.DeviceSettings.NotificationsEnabled ;

            var themeName = current.AppearanceSettings.ThemeName ;

            if ( Enum.TryParse<ApplicationTheme> ( themeName , out var parsedTheme ) &&
                 parsedTheme is ApplicationTheme.Light or
                                 ApplicationTheme.Dark or
                                 ApplicationTheme.HighContrast )
            {
                model.CurrentTheme = parsedTheme ;
            }
            else
            {
                model.CurrentTheme = ApplicationTheme.Light ;
            }

            if ( ! string.Equals ( themeSwitcher.CurrentThemeName , themeName , StringComparison.Ordinal ) )
            {
                themeSwitcher.ChangeTheme ( current.AppearanceSettings.ThemeName ) ;
            }
        }
        catch ( Exception ex )
        {
            logger.Error ( ex , "Failed to load settings" ) ;
            throw ;
        }
    }

    public async Task StoreSettingsAsync ( ISettingsViewModel model ,
                                           CancellationToken  token )
    {
        if ( model == null ) throw new ArgumentNullException ( nameof ( model ) ) ;

        var lockChanged     = HasParentalLockChanged ( model ) ;
        var advancedChanged = HaveAdvancedSettingsChanged ( model ) ;
        var anyChanged      = HaveAnySettingsChanged ( model ) || lockChanged || advancedChanged ;

        if ( ! anyChanged )
        {
            logger.Debug ( "No settings changes detected. Skipping save and notifications." ) ;
            return ;
        }

        UpdateCurrentSettings ( model ) ;

        await DoStoreSettingsAsync ( advancedChanged ,
                                     lockChanged ,
                                     token ) ;
    }

    public bool HasParentalLockChanged ( ISettingsViewModel model )
    {
        var settings = settingsManager.CurrentSettings ;

        return settings.DeviceSettings.DeviceLocked != model.ParentalLock ;
    }

    public bool HaveAdvancedSettingsChanged ( ISettingsViewModel model )
    {
        var settings = settingsManager.CurrentSettings ;

        var newDeviceName    = nameConverter.DefaultIfEmpty ( model.DeskName ) ;
        var newDeviceAddress = addressConverter.DefaultIfEmpty ( model.DeskAddress ) ;

        return settings.DeviceSettings.DeviceName    != newDeviceName ||
               settings.DeviceSettings.DeviceAddress != newDeviceAddress ;
    }

    public void UpdateCurrentSettings ( ISettingsViewModel model )
    {
        var settings = settingsManager.CurrentSettings ;

        var newDeviceName           = nameConverter.DefaultIfEmpty ( model.DeskName ) ;
        var newDeviceAddress        = addressConverter.DefaultIfEmpty ( model.DeskAddress ) ;
        var newDeviceLocked         = model.ParentalLock ;
        var newNotificationsEnabled = model.Notifications ;

        settings.HeightSettings.StandingHeightInCm = toUIntConverter.ConvertToUInt ( model.Standing ,
                                                                                     Constants.DefaultHeightStandingInCm ) ;
        settings.HeightSettings.SeatingHeightInCm  = toUIntConverter.ConvertToUInt ( model.Seating ,
                                                                                     Constants.DefaultHeightSeatingInCm ) ;
        settings.HeightSettings.Custom1HeightInCm  = toUIntConverter.ConvertToUInt ( model.Custom1 ,
                                                                                     Constants.DefaultHeightStandingInCm ) ;
        settings.HeightSettings.Custom2HeightInCm  = toUIntConverter.ConvertToUInt ( model.Custom2 ,
                                                                                     Constants.DefaultHeightSeatingInCm ) ;
        settings.HeightSettings.LastKnownDeskHeight = model.LastKnownDeskHeight ;

        settings.HeightSettings.StandingIsVisibleInContextMenu = model.StandingIsVisibleInContextMenu ;
        settings.HeightSettings.SeatingIsVisibleInContextMenu  = model.SeatingIsVisibleInContextMenu ;
        settings.HeightSettings.Custom1IsVisibleInContextMenu  = model.Custom1IsVisibleInContextMenu ;
        settings.HeightSettings.Custom2IsVisibleInContextMenu  = model.Custom2IsVisibleInContextMenu ;
        settings.DeviceSettings.StopIsVisibleInContextMenu     = model.StopIsVisibleInContextMenu ;

        settings.DeviceSettings.DeviceName           = newDeviceName ;
        settings.DeviceSettings.DeviceAddress        = newDeviceAddress ;
        settings.DeviceSettings.DeviceLocked         = newDeviceLocked ;
        settings.DeviceSettings.NotificationsEnabled = newNotificationsEnabled ;

        settings.AppearanceSettings.ThemeName = themeSwitcher.CurrentThemeName ;
    }

    public void ChangeTheme ( string parameter ) =>
        themeSwitcher.ChangeTheme ( parameter ) ;

    private async Task DoStoreSettingsAsync ( bool              advancedChanged ,
                                              bool              lockChanged ,
                                              CancellationToken token )
    {
        try
        {
            logger.Debug ( "Storing new settings: {SettingsManagerCurrentSettings}" ,
                           settingsManager.CurrentSettings ) ;

            await settingsManager.SaveAsync ( token ).ConfigureAwait ( false ) ;

            if ( advancedChanged )
            {
                AdvancedSettingsChanged ( advancedChanged ) ;
            }

            if ( lockChanged )
            {
                LockChanged ( settingsManager.CurrentSettings ) ;
            }
        }
        catch ( Exception e )
        {
            logger.Error ( e ,
                           "Failed to store settings" ) ;
        }
    }

    private void LockChanged ( ISettings settings )
    {
        logger.Information ( "Advanced Locked settings have changed..." ) ;

        settingsChanges.LockSettingsChanged.OnNext ( settings.DeviceSettings.DeviceLocked ) ;
    }

    private void AdvancedSettingsChanged ( bool advancedChanged )
    {
        logger.Information ( "Advanced settings have changed, reconnecting..." ) ;

        settingsChanges.AdvancedSettingsChanged.OnNext ( advancedChanged ) ;
    }

    private bool HaveAnySettingsChanged ( ISettingsViewModel model )
    {
        var current = settingsManager.CurrentSettings ;

        bool heightChanged =
            current.HeightSettings.DeskMinHeightInCm != model.MinHeight ||
            current.HeightSettings.DeskMaxHeightInCm != model.MaxHeight ||
            current.HeightSettings.StandingHeightInCm != toUIntConverter.ConvertToUInt ( model.Standing , Constants.DefaultHeightStandingInCm ) ||
            current.HeightSettings.SeatingHeightInCm  != toUIntConverter.ConvertToUInt ( model.Seating  , Constants.DefaultHeightSeatingInCm  ) ||
            current.HeightSettings.Custom1HeightInCm  != toUIntConverter.ConvertToUInt ( model.Custom1  , Constants.DefaultHeightStandingInCm ) ||
            current.HeightSettings.Custom2HeightInCm  != toUIntConverter.ConvertToUInt ( model.Custom2  , Constants.DefaultHeightSeatingInCm  ) ||
            current.HeightSettings.LastKnownDeskHeight != model.LastKnownDeskHeight ;

        bool visibilityChanged =
            current.HeightSettings.StandingIsVisibleInContextMenu != model.StandingIsVisibleInContextMenu ||
            current.HeightSettings.SeatingIsVisibleInContextMenu  != model.SeatingIsVisibleInContextMenu  ||
            current.HeightSettings.Custom1IsVisibleInContextMenu  != model.Custom1IsVisibleInContextMenu  ||
            current.HeightSettings.Custom2IsVisibleInContextMenu  != model.Custom2IsVisibleInContextMenu  ||
            current.DeviceSettings.StopIsVisibleInContextMenu     != model.StopIsVisibleInContextMenu ;

        bool deviceChanged =
            current.DeviceSettings.NotificationsEnabled != model.Notifications ||
            current.DeviceSettings.DeviceLocked         != model.ParentalLock ;

        bool themeChanged =
            ! string.Equals ( current.AppearanceSettings.ThemeName , themeSwitcher.CurrentThemeName , StringComparison.Ordinal ) ;

        return heightChanged || visibilityChanged || deviceChanged || themeChanged ;
    }
}