using Idasen.BluetoothLE.Linak.Control ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;
using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Utils ;

public class SettingsSynchronizer (
    ILogger                        logger ,
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
        ArgumentNullException.ThrowIfNull ( model ) ;

        try
        {
            logger.Debug ( "LoadSettingsAsync settings" ) ;

            await settingsManager.LoadAsync ( token ).ConfigureAwait ( false ) ;

            ApplySettingsToModel ( model ,
                                   settingsManager.CurrentSettings ) ;

            // Theme application moved to the UI layer (SettingsViewModel) to ensure it runs on the UI thread.

            StoppingHeightCalculatorSettings.MaxSpeedToStopMovement = model.MaxSpeedToStopMovement ;
        }
        catch ( Exception ex )
        {
            logger.Error ( ex ,
                           "Failed to load settings! Using default settings." ) ;

            await settingsManager.ResetSettingsAsync ( token ) ;

            ApplySettingsToModel ( model ,
                                   settingsManager.CurrentSettings ) ;
        }
    }

    public async Task StoreSettingsAsync ( ISettingsViewModel model ,
                                           CancellationToken  token )
    {
        ArgumentNullException.ThrowIfNull ( model ) ;

        var lockChanged     = HasParentalLockChanged ( model ) ;
        var advancedChanged = HaveAdvancedSettingsChanged ( model ) ;
        var hotkeyChanged   = HaveHotkeySettingsChanged ( model ) ;
        var anyChanged      = HaveAnySettingsChanged ( model ) || lockChanged || advancedChanged || hotkeyChanged ;

        logger.Debug ( "Settings change check - Lock: {LockChanged}, Advanced: {AdvancedChanged}, Hotkey: {HotkeyChanged}, Any: {AnyChanged}" ,
                       lockChanged ,
                       advancedChanged ,
                       hotkeyChanged ,
                       anyChanged ) ;

        if ( ! anyChanged )
        {
            logger.Debug ( "No settings changes detected. Skipping save and notifications." ) ;

            return ;
        }

        UpdateCurrentSettings ( model ) ;

        await DoStoreSettingsAsync ( advancedChanged ,
                                     lockChanged ,
                                     hotkeyChanged ,
                                     token ).ConfigureAwait ( false ) ;
    }

    public void ChangeTheme ( string parameter )
    {
        themeSwitcher.ChangeTheme ( parameter ) ;
    }

    private void ApplySettingsToModel ( ISettingsViewModel model ,
                                        ISettings current )
    {
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
        model.MaxSpeedToStopMovement = current.DeviceSettings.MaxSpeedToStopMovement > 0
                                           ? current.DeviceSettings.MaxSpeedToStopMovement
                                           : StoppingHeightCalculatorSettings.MaxSpeedToStopMovement ;

        model.GlobalHotkeysEnabled = current.HotkeySettings.GlobalHotkeysEnabled ;

        var themeName = current.AppearanceSettings.ThemeName ;
        model.CurrentTheme = ParseThemeName ( themeName ) ;
    }

    public bool HasParentalLockChanged ( ISettingsViewModel model )
    {
        var settings = settingsManager.CurrentSettings ;

        return settings.DeviceSettings.DeviceLocked != model.ParentalLock ;
    }

    public bool HaveAdvancedSettingsChanged ( ISettingsViewModel model )
    {
        var settings = settingsManager.CurrentSettings ;

        var (newDeviceName , newDeviceAddress) = GetNormalizedDevice ( model ) ;

        return settings.DeviceSettings.DeviceName    != newDeviceName ||
               settings.DeviceSettings.DeviceAddress != newDeviceAddress ;
    }

    public bool HaveHotkeySettingsChanged ( ISettingsViewModel model )
    {
        var settings = settingsManager.CurrentSettings ;

        return settings.HotkeySettings.GlobalHotkeysEnabled != model.GlobalHotkeysEnabled ;
    }

    public void UpdateCurrentSettings ( ISettingsViewModel model )
    {
        var settings = settingsManager.CurrentSettings ;

        var (newDeviceName , newDeviceAddress) = GetNormalizedDevice ( model ) ;
        var newDeviceLocked         = model.ParentalLock ;
        var newNotificationsEnabled = model.Notifications ;

        settings.HeightSettings.StandingHeightInCm = toUIntConverter.ConvertToUInt ( model.Standing ,
                                                                                     Constants
                                                                                        .DefaultHeightStandingInCm ) ;
        settings.HeightSettings.SeatingHeightInCm = toUIntConverter.ConvertToUInt ( model.Seating ,
                                                                                    Constants
                                                                                       .DefaultHeightSeatingInCm ) ;
        settings.HeightSettings.Custom1HeightInCm = toUIntConverter.ConvertToUInt ( model.Custom1 ,
                                                                                    Constants
                                                                                       .DefaultHeightStandingInCm ) ;
        settings.HeightSettings.Custom2HeightInCm = toUIntConverter.ConvertToUInt ( model.Custom2 ,
                                                                                    Constants
                                                                                       .DefaultHeightSeatingInCm ) ;
        settings.HeightSettings.LastKnownDeskHeight = model.LastKnownDeskHeight ;

        settings.HeightSettings.StandingIsVisibleInContextMenu = model.StandingIsVisibleInContextMenu ;
        settings.HeightSettings.SeatingIsVisibleInContextMenu  = model.SeatingIsVisibleInContextMenu ;
        settings.HeightSettings.Custom1IsVisibleInContextMenu  = model.Custom1IsVisibleInContextMenu ;
        settings.HeightSettings.Custom2IsVisibleInContextMenu  = model.Custom2IsVisibleInContextMenu ;
        settings.DeviceSettings.StopIsVisibleInContextMenu     = model.StopIsVisibleInContextMenu ;

        settings.DeviceSettings.DeviceName             = newDeviceName ;
        settings.DeviceSettings.DeviceAddress          = newDeviceAddress ;
        settings.DeviceSettings.DeviceLocked           = newDeviceLocked ;
        settings.DeviceSettings.NotificationsEnabled   = newNotificationsEnabled ;
        settings.DeviceSettings.MaxSpeedToStopMovement = model.MaxSpeedToStopMovement ;

        settings.HotkeySettings.GlobalHotkeysEnabled = model.GlobalHotkeysEnabled ;

        settings.AppearanceSettings.ThemeName = themeSwitcher.CurrentThemeName ;

        StoppingHeightCalculatorSettings.MaxSpeedToStopMovement = model.MaxSpeedToStopMovement ;
    }

    private async Task DoStoreSettingsAsync ( bool              advancedChanged ,
                                              bool              lockChanged ,
                                              bool              hotkeyChanged ,
                                              CancellationToken token )
    {
        try
        {
            logger.Debug ( "Storing new settings: {SettingsManagerCurrentSettings}" ,
                           settingsManager.CurrentSettings ) ;

            await settingsManager.SaveAsync ( token ).ConfigureAwait ( false ) ;

            if ( advancedChanged ) AdvancedSettingsChanged ( advancedChanged ) ;

            if ( lockChanged ) LockChanged ( settingsManager.CurrentSettings ) ;

            if ( hotkeyChanged ) HotkeyChanged ( settingsManager.CurrentSettings ) ;
        }
        catch ( Exception e )
        {
            logger.Error ( e ,
                           "Failed to store settings" ) ;

            throw new InvalidOperationException ( "Failed to store settings" ) ;
        }
    }

    private void LockChanged ( ISettings settings )
    {
        logger.Information ( "Advanced Locked settings have changed..." ) ;

        settingsChanges.LockSettingsChanged.OnNext ( settings.DeviceSettings.DeviceLocked ) ;
    }

    private void HotkeyChanged ( ISettings settings )
    {
        logger.Information ( "Hotkey settings have changed. GlobalHotkeysEnabled: {Enabled}" ,
                             settings.HotkeySettings.GlobalHotkeysEnabled ) ;

        settingsChanges.HotkeySettingsChanged.OnNext ( settings.HotkeySettings.GlobalHotkeysEnabled ) ;

        logger.Debug ( "Published HotkeySettingsChanged notification with value: {Enabled}" ,
                       settings.HotkeySettings.GlobalHotkeysEnabled ) ;
    }

    private void AdvancedSettingsChanged ( bool advancedChanged )
    {
        logger.Information ( "Advanced settings have changed, reconnecting..." ) ;

        settingsChanges.AdvancedSettingsChanged.OnNext ( advancedChanged ) ;
    }

    private bool HaveAnySettingsChanged ( ISettingsViewModel model )
    {
        var current = settingsManager.CurrentSettings ;

        // cache conversions
        var newStanding = toUIntConverter.ConvertToUInt ( model.Standing ,
                                                          Constants.DefaultHeightStandingInCm ) ;
        var newSeating = toUIntConverter.ConvertToUInt ( model.Seating ,
                                                         Constants.DefaultHeightSeatingInCm ) ;
        var newCustom1 = toUIntConverter.ConvertToUInt ( model.Custom1 ,
                                                         Constants.DefaultHeightStandingInCm ) ;
        var newCustom2 = toUIntConverter.ConvertToUInt ( model.Custom2 ,
                                                         Constants.DefaultHeightSeatingInCm ) ;

        var heightChanged =
            current.HeightSettings.DeskMinHeightInCm   != model.MinHeight ||
            current.HeightSettings.DeskMaxHeightInCm   != model.MaxHeight ||
            current.HeightSettings.StandingHeightInCm  != newStanding     ||
            current.HeightSettings.SeatingHeightInCm   != newSeating      ||
            current.HeightSettings.Custom1HeightInCm   != newCustom1      ||
            current.HeightSettings.Custom2HeightInCm   != newCustom2      ||
            current.HeightSettings.LastKnownDeskHeight != model.LastKnownDeskHeight ;

        var visibilityChanged =
            current.HeightSettings.StandingIsVisibleInContextMenu != model.StandingIsVisibleInContextMenu ||
            current.HeightSettings.SeatingIsVisibleInContextMenu  != model.SeatingIsVisibleInContextMenu  ||
            current.HeightSettings.Custom1IsVisibleInContextMenu  != model.Custom1IsVisibleInContextMenu  ||
            current.HeightSettings.Custom2IsVisibleInContextMenu  != model.Custom2IsVisibleInContextMenu  ||
            current.DeviceSettings.StopIsVisibleInContextMenu     != model.StopIsVisibleInContextMenu ;

        var deviceChanged =
            current.DeviceSettings.NotificationsEnabled   != model.Notifications ||
            current.DeviceSettings.DeviceLocked           != model.ParentalLock  ||
            current.DeviceSettings.MaxSpeedToStopMovement != model.MaxSpeedToStopMovement ;

        var themeChanged =
            ! string.Equals ( current.AppearanceSettings.ThemeName ,
                              themeSwitcher.CurrentThemeName ,
                              StringComparison.Ordinal ) ;

        var hotkeyChanged =
            current.HotkeySettings.GlobalHotkeysEnabled != model.GlobalHotkeysEnabled ;

        return heightChanged || visibilityChanged || deviceChanged || themeChanged || hotkeyChanged ;
    }

    private static ApplicationTheme ParseThemeName ( string themeName )
    {
        return Enum.TryParse < ApplicationTheme > ( themeName ,
                                                    out var parsedTheme ) &&
               parsedTheme is ApplicationTheme.Light or ApplicationTheme.Dark or ApplicationTheme.HighContrast
                   ? parsedTheme
                   : ApplicationTheme.Light ;
    }

    private ( string Name , ulong Address ) GetNormalizedDevice ( ISettingsViewModel model )
    {
        return ( nameConverter.DefaultIfEmpty ( model.DeskName ) ,
                 addressConverter.DefaultIfEmpty ( model.DeskAddress ) ) ;
    }
}