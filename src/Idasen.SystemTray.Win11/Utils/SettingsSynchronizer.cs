using Idasen.SystemTray.Win11.Interfaces;
using Idasen.SystemTray.Win11.TraySettings ;
using Serilog;
using Wpf.Ui.Appearance;

namespace Idasen.SystemTray.Win11.Utils ;

public class SettingsSynchronizer ( ILogger                        logger ,
                                    ILoggingSettingsManager        settingsManager ,
                                    IDoubleToUIntConverter         toUIntConverter ,
                                    IDeviceNameConverter           nameConverter ,
                                    IDeviceAddressToULongConverter addressConverter ,
                                    INotifySettingsChanges         settingsChanges ,
                                    IThemeSwitcher themeSwitcher ) : ISettingsSynchronizer
{
    public async Task LoadSettingsAsync ( ISettingsViewModel model ,
                                          CancellationToken token )
    {
        logger.Debug ( "LoadSettingsAsync settings" ) ;

        await settingsManager.LoadAsync ( token ) ;

        var current = settingsManager.CurrentSettings ;

        model.MinHeight = current.HeightSettings.DeskMinHeightInCm ;
        model.MaxHeight = current.HeightSettings.DeskMaxHeightInCm ;
        model.Standing  = current.HeightSettings.StandingHeightInCm;
        model.Seating   = current.HeightSettings.SeatingHeightInCm ;
        model.Custom1   = current.HeightSettings.Custom1HeightInCm ;
        model.Custom2   = current.HeightSettings.Custom2HeightInCm ;

        model.StandingIsVisibleInContextMenu = current.HeightSettings.StandingIsVisibleInContextMenu;
        model.SeatingIsVisibleInContextMenu  = current.HeightSettings.SeatingIsVisibleInContextMenu;
        model.Custom1IsVisibleInContextMenu  = current.HeightSettings.Custom1IsVisibleInContextMenu;
        model.Custom2IsVisibleInContextMenu  = current.HeightSettings.Custom2IsVisibleInContextMenu;
        model.StopIsVisibleInContextMenu     = current.DeviceSettings.StopIsVisibleInContextMenu;

        model.LastKnownDeskHeight = current.HeightSettings.LastKnownDeskHeight ;
        model.DeskName            = nameConverter.EmptyIfDefault ( current.DeviceSettings.DeviceName ) ;
        model.DeskAddress         = addressConverter.EmptyIfDefault ( current.DeviceSettings.DeviceAddress ) ;
        model.ParentalLock        = current.DeviceSettings.DeviceLocked ;
        model.Notifications       = current.DeviceSettings.NotificationsEnabled ;

        var themeName = current.AppearanceSettings.ThemeName;

        if (Enum.TryParse<ApplicationTheme>(themeName, out var parsedTheme) &&
            parsedTheme is ApplicationTheme.Light or 
                           ApplicationTheme.Dark or 
                           ApplicationTheme.HighContrast)
        {
            model.CurrentTheme = parsedTheme;
        }
        else
        {
            model.CurrentTheme = ApplicationTheme.Light;
        }

        themeSwitcher.ChangeTheme ( current.AppearanceSettings.ThemeName );

        HeightSettingsChanged( current.HeightSettings );
    }

    public async Task StoreSettingsAsync (ISettingsViewModel model ,
                                          CancellationToken token )
    {
        var lockChanged           = HasParentalLockChanged ( model ) ;
        var advancedChanged       = HaveAdvancedSettingsChanged ( model ) ;
        var heightSettingsChanged = HaveIsVisibleInContextMenuChanged ( model ); // todo heights are not checked at the moment
        var stopChanged           = HasStopIsVisibleInContextMenuChanged ( model ) ;

        UpdateCurrentSettings ( model ) ;

        await DoStoreSettingsAsync ( advancedChanged ,
                                     lockChanged ,
                                     heightSettingsChanged ,
                                     stopChanged ,
                                     token ) ;
    }

    public bool HasParentalLockChanged (ISettingsViewModel model )
    {
        var settings = settingsManager.CurrentSettings ;

        return settings.DeviceSettings.DeviceLocked != model.ParentalLock ;
    }

    public bool HaveAdvancedSettingsChanged (ISettingsViewModel model )
    {
        var settings = settingsManager.CurrentSettings ;

        var newDeviceName    = nameConverter.DefaultIfEmpty ( model.DeskName ) ;
        var newDeviceAddress = addressConverter.DefaultIfEmpty ( model.DeskAddress ) ;

        return settings.DeviceSettings.DeviceName    != newDeviceName ||
               settings.DeviceSettings.DeviceAddress != newDeviceAddress ;
    }

    private bool HaveIsVisibleInContextMenuChanged(ISettingsViewModel model)
    {
        var settings = settingsManager.CurrentSettings;

        return settings.HeightSettings.StandingIsVisibleInContextMenu != model.StandingIsVisibleInContextMenu ||
               settings.HeightSettings.SeatingIsVisibleInContextMenu  != model.SeatingIsVisibleInContextMenu  ||
               settings.HeightSettings.Custom1IsVisibleInContextMenu  != model.Custom1IsVisibleInContextMenu  ||
               settings.HeightSettings.Custom2IsVisibleInContextMenu  != model.Custom2IsVisibleInContextMenu ;
    }

    public bool HasStopIsVisibleInContextMenuChanged(ISettingsViewModel model)
    {
        var settings = settingsManager.CurrentSettings;

        return settings.DeviceSettings.StopIsVisibleInContextMenu != model.StopIsVisibleInContextMenu;
    }

    public void UpdateCurrentSettings (ISettingsViewModel model )
    {
        var settings = settingsManager.CurrentSettings ;

        var newDeviceName           = nameConverter.DefaultIfEmpty ( model.DeskName ) ;
        var newDeviceAddress        = addressConverter.DefaultIfEmpty ( model.DeskAddress ) ;
        var newDeviceLocked         = model.ParentalLock ;
        var newNotificationsEnabled = model.Notifications ;

        settings.HeightSettings.StandingHeightInCm = toUIntConverter.ConvertToUInt ( model.Standing ,
                                                                                     Constants.DefaultHeightStandingInCm ) ;
        settings.HeightSettings.SeatingHeightInCm = toUIntConverter.ConvertToUInt ( model.Seating ,
                                                                                    Constants.DefaultHeightSeatingInCm ) ;
        settings.HeightSettings.Custom1HeightInCm = toUIntConverter.ConvertToUInt ( model.Custom1 ,
                                                                                      Constants.DefaultHeightStandingInCm ) ;
        settings.HeightSettings.Custom2HeightInCm    = toUIntConverter.ConvertToUInt ( model.Custom2 ,
                                                                                     Constants.DefaultHeightSeatingInCm ) ;
        settings.HeightSettings.LastKnownDeskHeight  = model.LastKnownDeskHeight ;

        settings.HeightSettings.StandingIsVisibleInContextMenu = model.StandingIsVisibleInContextMenu ;
        settings.HeightSettings.SeatingIsVisibleInContextMenu  = model.SeatingIsVisibleInContextMenu ;
        settings.HeightSettings.Custom1IsVisibleInContextMenu  = model.Custom1IsVisibleInContextMenu ;
        settings.HeightSettings.Custom2IsVisibleInContextMenu  = model.Custom2IsVisibleInContextMenu ;
        settings.DeviceSettings.StopIsVisibleInContextMenu     = model.StopIsVisibleInContextMenu ;

        settings.DeviceSettings.DeviceName                     = newDeviceName ;
        settings.DeviceSettings.DeviceAddress                  = newDeviceAddress ;
        settings.DeviceSettings.DeviceLocked                   = newDeviceLocked ;
        settings.DeviceSettings.NotificationsEnabled           = newNotificationsEnabled ;

        settings.AppearanceSettings.ThemeName = themeSwitcher.CurrentThemeName;
    }

    public void ChangeTheme(string parameter) => 
        themeSwitcher.ChangeTheme(parameter);

    private async Task DoStoreSettingsAsync ( bool              advancedChanged ,
                                              bool              lockChanged ,
                                              bool              heightSettingsChanged ,
                                              bool              stopChanged ,
                                              CancellationToken token )
    {
        try
        {
            logger.Debug ( "Storing new settings: {SettingsManagerCurrentSettings}" ,
                           settingsManager.CurrentSettings ) ;

            await settingsManager.SaveAsync ( token ) ;

            if ( advancedChanged )
            {
                AdvancedSettingsChanged ( advancedChanged ) ;
            }

            if ( lockChanged )
            {
                LockChanged ( settingsManager.CurrentSettings ) ;
            }

            if ( heightSettingsChanged )
            {
                HeightSettingsChanged ( settingsManager.CurrentSettings.HeightSettings ) ;
            }

            if (stopChanged)
            {
                StopChanged(settingsManager.CurrentSettings);
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

    private void HeightSettingsChanged(HeightSettings settings)
    {
        logger.Information("Height settings have changed..."); 

        settingsChanges.HeightSettingsChanged.OnNext(settings);
    }

    private void StopChanged(ISettings settings)
    {
        logger.Information("Stop settings have changed...");

        settingsChanges.StopChanged.OnNext(settings);
    }
}