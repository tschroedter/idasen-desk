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

            // Notify subscribers that height settings have been loaded
            HeightChanged ( settingsManager.CurrentSettings ) ;
        }
        catch ( Exception ex )
        {
            logger.Error ( ex ,
                           "Failed to load settings! Using default settings." ) ;

            await settingsManager.ResetSettingsAsync ( token ) ;

            ApplySettingsToModel ( model ,
                                   settingsManager.CurrentSettings ) ;

            // Notify subscribers that height settings have been loaded
            HeightChanged ( settingsManager.CurrentSettings ) ;
        }
    }

    public async Task StoreSettingsAsync ( ISettingsViewModel model ,
                                           CancellationToken  token )
    {
        ArgumentNullException.ThrowIfNull ( model ) ;

        var lockChanged     = HasParentalLockChanged ( model ) ;
        var advancedChanged = HaveAdvancedSettingsChanged ( model ) ;
        var hotkeyChanged   = HaveHotkeySettingsChanged ( model ) ;
        var heightChanged   = HaveHeightSettingsChanged ( model ) ;
        var anyChanged      = HaveAnySettingsChanged ( model ) || lockChanged || advancedChanged || hotkeyChanged || heightChanged ;

        logger.Debug ( "Settings change check - Lock: {LockChanged}, Advanced: {AdvancedChanged}, Hotkey: {HotkeyChanged}, Height: {HeightChanged}, Any: {AnyChanged}" ,
                       lockChanged ,
                       advancedChanged ,
                       hotkeyChanged ,
                       heightChanged ,
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
                                     heightChanged ,
                                     token ).ConfigureAwait ( false ) ;
    }

    public void ChangeTheme ( string parameter )
    {
        themeSwitcher.ChangeTheme ( parameter ) ;
    }

    private void ApplySettingsToModel ( ISettingsViewModel model ,
                                        ISettings current )
    {
        model.MinHeight    = current.HeightSettings.DeskMinHeightInCm ;
        model.MaxHeight    = current.HeightSettings.DeskMaxHeightInCm ;
        model.Standing     = current.HeightSettings.StandingHeightInCm ;
        model.StandingName = current.HeightSettings.StandingName ;
        model.Seating      = current.HeightSettings.SeatingHeightInCm ;
        model.SeatingName  = current.HeightSettings.SeatingName ;
        model.Custom1      = current.HeightSettings.Custom1HeightInCm ;
        model.Custom1Name  = current.HeightSettings.Custom1Name ;
        model.Custom2      = current.HeightSettings.Custom2HeightInCm ;
        model.Custom2Name  = current.HeightSettings.Custom2Name ;

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

        model.StandingKey       = current.HotkeySettings.StandingKey ;
        model.StandingModifiers = current.HotkeySettings.StandingModifiers ;
        model.SeatingKey        = current.HotkeySettings.SeatingKey ;
        model.SeatingModifiers  = current.HotkeySettings.SeatingModifiers ;
        model.Custom1Key        = current.HotkeySettings.Custom1Key ;
        model.Custom1Modifiers  = current.HotkeySettings.Custom1Modifiers ;
        model.Custom2Key        = current.HotkeySettings.Custom2Key ;
        model.Custom2Modifiers  = current.HotkeySettings.Custom2Modifiers ;

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

        return settings.HotkeySettings.GlobalHotkeysEnabled != model.GlobalHotkeysEnabled ||
               settings.HotkeySettings.StandingKey          != model.StandingKey          ||
               settings.HotkeySettings.StandingModifiers    != model.StandingModifiers    ||
               settings.HotkeySettings.SeatingKey           != model.SeatingKey           ||
               settings.HotkeySettings.SeatingModifiers     != model.SeatingModifiers     ||
               settings.HotkeySettings.Custom1Key           != model.Custom1Key           ||
               settings.HotkeySettings.Custom1Modifiers     != model.Custom1Modifiers     ||
               settings.HotkeySettings.Custom2Key           != model.Custom2Key           ||
               settings.HotkeySettings.Custom2Modifiers     != model.Custom2Modifiers ;
    }

    public bool HaveHeightSettingsChanged ( ISettingsViewModel model )
    {
        var settings = settingsManager.CurrentSettings ;

        var newStanding     = toUIntConverter.ConvertToUInt ( model.Standing , Constants.DefaultHeightStandingInCm ) ;
        var newSeating      = toUIntConverter.ConvertToUInt ( model.Seating , Constants.DefaultHeightSeatingInCm ) ;
        var newCustom1      = toUIntConverter.ConvertToUInt ( model.Custom1 , Constants.DefaultHeightStandingInCm ) ;
        var newCustom2      = toUIntConverter.ConvertToUInt ( model.Custom2 , Constants.DefaultHeightSeatingInCm ) ;
        var newStandingName = string.IsNullOrWhiteSpace ( model.StandingName ) ? Constants.DefaultStandingName : model.StandingName ;
        var newSeatingName  = string.IsNullOrWhiteSpace ( model.SeatingName ) ? Constants.DefaultSeatingName : model.SeatingName ;
        var newCustom1Name  = string.IsNullOrWhiteSpace ( model.Custom1Name ) ? Constants.DefaultCustom1Name : model.Custom1Name ;
        var newCustom2Name  = string.IsNullOrWhiteSpace ( model.Custom2Name ) ? Constants.DefaultCustom2Name : model.Custom2Name ;

        return settings.HeightSettings.DeskMinHeightInCm            != model.MinHeight ||
               settings.HeightSettings.DeskMaxHeightInCm            != model.MaxHeight ||
               settings.HeightSettings.StandingHeightInCm           != newStanding ||
               settings.HeightSettings.StandingName                 != newStandingName ||
               settings.HeightSettings.SeatingHeightInCm            != newSeating ||
               settings.HeightSettings.SeatingName                  != newSeatingName ||
               settings.HeightSettings.Custom1HeightInCm            != newCustom1 ||
               settings.HeightSettings.Custom1Name                  != newCustom1Name ||
               settings.HeightSettings.Custom2HeightInCm            != newCustom2 ||
               settings.HeightSettings.Custom2Name                  != newCustom2Name ||
               settings.HeightSettings.LastKnownDeskHeight          != model.LastKnownDeskHeight ||
               settings.HeightSettings.StandingIsVisibleInContextMenu != model.StandingIsVisibleInContextMenu ||
               settings.HeightSettings.SeatingIsVisibleInContextMenu  != model.SeatingIsVisibleInContextMenu ||
               settings.HeightSettings.Custom1IsVisibleInContextMenu  != model.Custom1IsVisibleInContextMenu ||
               settings.HeightSettings.Custom2IsVisibleInContextMenu  != model.Custom2IsVisibleInContextMenu ;
    }

    public void UpdateCurrentSettings ( ISettingsViewModel model )
    {
        var settings = settingsManager.CurrentSettings ;

        var (newDeviceName , newDeviceAddress) = GetNormalizedDevice ( model ) ;
        var newDeviceLocked         = model.ParentalLock ;
        var newNotificationsEnabled = model.Notifications ;

        settings.HeightSettings.StandingHeightInCm = toUIntConverter.ConvertToUInt ( model.Standing ,
                                                                                     Constants.DefaultHeightStandingInCm ) ;
        settings.HeightSettings.StandingName = string.IsNullOrWhiteSpace ( model.StandingName ) ? Constants.DefaultStandingName : model.StandingName ;
        settings.HeightSettings.SeatingHeightInCm = toUIntConverter.ConvertToUInt ( model.Seating ,
                                                                                    Constants.DefaultHeightSeatingInCm ) ;
        settings.HeightSettings.SeatingName = string.IsNullOrWhiteSpace ( model.SeatingName ) ? Constants.DefaultSeatingName : model.SeatingName ;
        settings.HeightSettings.Custom1HeightInCm = toUIntConverter.ConvertToUInt ( model.Custom1 ,
                                                                                    Constants.DefaultHeightStandingInCm ) ;
        settings.HeightSettings.Custom1Name = string.IsNullOrWhiteSpace ( model.Custom1Name ) ? Constants.DefaultCustom1Name : model.Custom1Name ;
        settings.HeightSettings.Custom2HeightInCm = toUIntConverter.ConvertToUInt ( model.Custom2 ,
                                                                                    Constants.DefaultHeightSeatingInCm ) ;
        settings.HeightSettings.Custom2Name = string.IsNullOrWhiteSpace ( model.Custom2Name ) ? Constants.DefaultCustom2Name : model.Custom2Name ;
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
        settings.HotkeySettings.StandingKey          = model.StandingKey ;
        settings.HotkeySettings.StandingModifiers    = model.StandingModifiers ;
        settings.HotkeySettings.SeatingKey           = model.SeatingKey ;
        settings.HotkeySettings.SeatingModifiers     = model.SeatingModifiers ;
        settings.HotkeySettings.Custom1Key           = model.Custom1Key ;
        settings.HotkeySettings.Custom1Modifiers     = model.Custom1Modifiers ;
        settings.HotkeySettings.Custom2Key           = model.Custom2Key ;
        settings.HotkeySettings.Custom2Modifiers     = model.Custom2Modifiers ;

        settings.AppearanceSettings.ThemeName = themeSwitcher.CurrentThemeName ;

        StoppingHeightCalculatorSettings.MaxSpeedToStopMovement = model.MaxSpeedToStopMovement ;
    }

    private async Task DoStoreSettingsAsync ( bool              advancedChanged ,
                                              bool              lockChanged ,
                                              bool              hotkeyChanged ,
                                              bool              heightChanged ,
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

            if ( heightChanged ) HeightChanged ( settingsManager.CurrentSettings ) ;
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

    private void HeightChanged ( ISettings settings )
    {
        logger.Information ( "Height settings have changed." ) ;

        settingsChanges.HeightSettingsChanged.OnNext ( true ) ;

        logger.Debug ("Published HeightSettingsChanged notification. Settings = {Settings}",
                      settings) ;
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
        var newStandingName = string.IsNullOrWhiteSpace ( model.StandingName ) ? Constants.DefaultStandingName : model.StandingName ;
        var newSeatingName  = string.IsNullOrWhiteSpace ( model.SeatingName ) ? Constants.DefaultSeatingName : model.SeatingName ;
        var newCustom1Name  = string.IsNullOrWhiteSpace ( model.Custom1Name ) ? Constants.DefaultCustom1Name : model.Custom1Name ;
        var newCustom2Name  = string.IsNullOrWhiteSpace ( model.Custom2Name ) ? Constants.DefaultCustom2Name : model.Custom2Name ;

        var heightChanged =
            current.HeightSettings.DeskMinHeightInCm   != model.MinHeight ||
            current.HeightSettings.DeskMaxHeightInCm   != model.MaxHeight ||
            current.HeightSettings.StandingHeightInCm  != newStanding     ||
            current.HeightSettings.StandingName        != newStandingName ||
            current.HeightSettings.SeatingHeightInCm   != newSeating      ||
            current.HeightSettings.SeatingName         != newSeatingName ||
            current.HeightSettings.Custom1HeightInCm   != newCustom1      ||
            current.HeightSettings.Custom1Name         != newCustom1Name ||
            current.HeightSettings.Custom2HeightInCm   != newCustom2      ||
            current.HeightSettings.Custom2Name         != newCustom2Name ||
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
            current.HotkeySettings.GlobalHotkeysEnabled != model.GlobalHotkeysEnabled ||
            current.HotkeySettings.StandingKey          != model.StandingKey          ||
            current.HotkeySettings.StandingModifiers    != model.StandingModifiers    ||
            current.HotkeySettings.SeatingKey           != model.SeatingKey           ||
            current.HotkeySettings.SeatingModifiers     != model.SeatingModifiers     ||
            current.HotkeySettings.Custom1Key           != model.Custom1Key           ||
            current.HotkeySettings.Custom1Modifiers     != model.Custom1Modifiers     ||
            current.HotkeySettings.Custom2Key           != model.Custom2Key           ||
            current.HotkeySettings.Custom2Modifiers     != model.Custom2Modifiers ;

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