using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Serilog ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public class SettingsSynchronizer ( ILogger                        logger ,
                                    ILoggingSettingsManager        settingsManager ,
                                    IDoubleToUIntConverter         toUIntConverter ,
                                    IDeviceNameConverter           nameConverter ,
                                    IDeviceAddressToULongConverter addressConverter ,
                                    INotifySettingsChanges         settingsChanges ) : ISettingsSynchronizer
{
    public async Task LoadSettingsAsync ( SettingsViewModel model , CancellationToken token )
    {
        logger.Debug ( "LoadSettingsAsync settings" ) ;

        await settingsManager.LoadAsync ( token ) ;

        var current = settingsManager.CurrentSettings ;

        model.Standing            = current.HeightSettings.StandingHeightInCm ;
        model.MinHeight           = current.HeightSettings.DeskMinHeightInCm ;
        model.MaxHeight           = current.HeightSettings.DeskMaxHeightInCm ;
        model.Seating             = current.HeightSettings.SeatingHeightInCm ;
        model.LastKnownDeskHeight = current.HeightSettings.LastKnownDeskHeight ;
        model.DeskName            = nameConverter.EmptyIfDefault ( current.DeviceSettings.DeviceName ) ;
        model.DeskAddress         = addressConverter.EmptyIfDefault ( current.DeviceSettings.DeviceAddress ) ;
        model.ParentalLock        = current.DeviceSettings.DeviceLocked ;
        model.Notifications       = current.DeviceSettings.NotificationsEnabled ;
    }

    public async Task StoreSettingsAsync ( SettingsViewModel model , CancellationToken token )
    {
        var lockChanged     = HasParentalLockChanged ( model ) ;
        var advancedChanged = HaveAdvancedSettingsChanged ( model ) ;

        UpdateCurrentSettings ( model ) ;

        await DoStoreSettingsAsync ( advancedChanged ,
                                     lockChanged ,
                                     token ) ;
    }

    public bool HasParentalLockChanged ( SettingsViewModel model )
    {
        var settings = settingsManager.CurrentSettings ;

        return settings.DeviceSettings.DeviceLocked != model.ParentalLock ;
    }

    public bool HaveAdvancedSettingsChanged ( SettingsViewModel model )
    {
        var settings = settingsManager.CurrentSettings ;

        var newDeviceName    = nameConverter.DefaultIfEmpty ( model.DeskName ) ;
        var newDeviceAddress = addressConverter.DefaultIfEmpty ( model.DeskAddress ) ;

        return settings.DeviceSettings.DeviceName    != newDeviceName ||
               settings.DeviceSettings.DeviceAddress != newDeviceAddress ;
    }

    public void UpdateCurrentSettings ( SettingsViewModel model )
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
        settings.HeightSettings.LastKnownDeskHeight  = model.LastKnownDeskHeight ;
        settings.DeviceSettings.DeviceName           = newDeviceName ;
        settings.DeviceSettings.DeviceAddress        = newDeviceAddress ;
        settings.DeviceSettings.DeviceLocked         = newDeviceLocked ;
        settings.DeviceSettings.NotificationsEnabled = newNotificationsEnabled ;
    }

    private async Task DoStoreSettingsAsync ( bool              advancedChanged ,
                                              bool              lockChanged ,
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
}