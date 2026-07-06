using System.Diagnostics.CodeAnalysis ;

namespace Idasen.SystemTray.Win11.Utils ;

/// <summary>
///     Application configuration organized by concern.
///     Provides centralized, discoverable constants for application behavior, defaults, and timeouts.
/// </summary>
[ ExcludeFromCodeCoverage ]
public static class AppConfiguration
{
    /// <summary>
    ///     Application metadata and file names
    /// </summary>
    public static class Application
    {
        /// <summary>
        ///     Application name used for mutex, logging, and system identification
        /// </summary>
        public const string Name = "Idasen.SystemTray" ;

        /// <summary>
        ///     Default filename for persisted settings
        /// </summary>
        public const string SettingsFileName = "Settings.json" ;
    }

    /// <summary>
    ///     Default values for user-configurable settings
    /// </summary>
    public static class Defaults
    {
        /// <summary>
        ///     Default standing desk height in centimeters
        /// </summary>
        public const uint HeightStandingInCm = 120 ;

        /// <summary>
        ///     Default sitting desk height in centimeters
        /// </summary>
        public const uint HeightSeatingInCm = 65 ;

        /// <summary>
        ///     Minimum allowed desk height in centimeters
        /// </summary>
        public const uint DeskMinHeightInCm = 60 ;

        /// <summary>
        ///     Maximum allowed desk height in centimeters
        /// </summary>
        public const uint DeskMaxHeightInCm = 127 ;

        /// <summary>
        ///     Default Bluetooth device name for desk discovery
        /// </summary>
        public const string DeviceName = "Desk" ;

        /// <summary>
        ///     Default Bluetooth device address
        /// </summary>
        public const ulong DeviceAddress = 250635178951455u ;

        /// <summary>
        ///     Default device monitoring timeout in seconds.
        ///     Time to wait for device response before considering it disconnected.
        /// </summary>
        public const uint DeviceMonitoringTimeout = 600 ;

        /// <summary>
        ///     Default device lock state
        /// </summary>
        public const bool Locked = false ;

        /// <summary>
        ///     Default theme name for application appearance
        /// </summary>
        public const string AppearanceThemeName = "theme_light" ;

        /// <summary>
        ///     Default state for global hotkey registration
        /// </summary>
        public const bool GlobalHotkeysEnabled = true ;

        /// <summary>
        ///     Default state for desktop notifications
        /// </summary>
        public static readonly bool NotificationsEnabled = true ;
    }

    /// <summary>
    ///     Default hotkey configuration
    /// </summary>
    public static class Hotkeys
    {
        /// <summary>
        ///     Default hotkey modifiers for all desk position hotkeys
        /// </summary>
        public const string DefaultModifiers = "Control, Alt, Shift" ;

        /// <summary>
        ///     Default key for standing position hotkey
        /// </summary>
        public const string StandingKey = "Up" ;

        /// <summary>
        ///     Default key for sitting position hotkey
        /// </summary>
        public const string SeatingKey = "Down" ;

        /// <summary>
        ///     Default key for custom position 1 hotkey
        /// </summary>
        public const string Custom1Key = "Left" ;

        /// <summary>
        ///     Default key for custom position 2 hotkey
        /// </summary>
        public const string Custom2Key = "Right" ;

        /// <summary>
        ///     Default display name for standing position
        /// </summary>
        public const string StandingName = "Stand" ;

        /// <summary>
        ///     Default display name for sitting position
        /// </summary>
        public const string SeatingName = "Sit" ;

        /// <summary>
        ///     Default display name for custom position 1
        /// </summary>
        public const string Custom1Name = "Custom 1" ;

        /// <summary>
        ///     Default display name for custom position 2
        /// </summary>
        public const string Custom2Name = "Custom 2" ;
    }

    /// <summary>
    ///     Timeout configuration for various operations
    /// </summary>
    public static class Timeouts
    {
        /// <summary>
        ///     Timeout for UI Desk Manager initialization and settings operations.
        ///     Allows sufficient time for Bluetooth connection and settings load.
        /// </summary>
        public const int InitializationSeconds = 60 ;

        /// <summary>
        ///     Delay in seconds before clearing status bar info message.
        ///     Gives user time to read notification messages.
        /// </summary>
        public const int StatusBarInfoClearDelaySeconds = 10 ;

        /// <summary>
        ///     Throttle delay in milliseconds for auto-save settings.
        ///     Prevents excessive saves during rapid property changes.
        /// </summary>
        public const int SettingsAutoSaveThrottleMilliseconds = 300 ;

        /// <summary>
        ///     Throttle duration in seconds for desk height change notifications.
        ///     Prevents excessive updates during continuous desk movement.
        /// </summary>
        public const int HeightChangeThrottleSeconds = 1 ;

        /// <summary>
        ///     Timeout in milliseconds for Bluetooth connection health monitoring.
        ///     If no activity is detected within this period, the connection is considered stale
        ///     and automatic reconnection is attempted. Default: 60000ms (60 seconds).
        /// </summary>
        public const int ConnectionMonitorTimeoutMilliseconds = 60000 ;
    }

    /// <summary>
    ///     UI and display configuration
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class UI
    {
        /// <summary>
        ///     Conversion factor between desk height in millimeters and centimeters.
        ///     Desk hardware reports height in millimeters, UI displays in centimeters.
        /// </summary>
        public const uint DeskHeightFactor = 100 ;
    }
}

// Legacy Constants class - marked obsolete for backward compatibility during migration
#pragma warning disable CS0618 // Type or member is obsolete
[ Obsolete ( "Use AppConfiguration instead. This class will be removed in a future version." ) ]
public static class Constants
{
    [ Obsolete ( "Use AppConfiguration.Application.Name" ) ]
    public const string ApplicationName = "Idasen.SystemTray" ;

    [ Obsolete ( "Use AppConfiguration.Application.SettingsFileName" ) ]
    public const string SettingsFileName = "Settings.json" ;

    [ Obsolete ( "Use AppConfiguration.Defaults.HeightStandingInCm" ) ]
    public const uint DefaultHeightStandingInCm = 120 ;

    [ Obsolete ( "Use AppConfiguration.Defaults.HeightSeatingInCm" ) ]
    public const uint DefaultHeightSeatingInCm = 65 ;

    [ Obsolete ( "Use AppConfiguration.Defaults.DeviceName" ) ]
    public const string DefaultDeviceName = "Desk" ;

    [ Obsolete ( "Use AppConfiguration.Defaults.DeviceAddress" ) ]
    public const ulong DefaultDeviceAddress = 250635178951455u ;

    [ Obsolete ( "Use AppConfiguration.Defaults.DeviceMonitoringTimeout" ) ]
    public const uint DefaultDeviceMonitoringTimeout = 600 ;

    [ Obsolete ( "Use AppConfiguration.Defaults.Locked" ) ]
    public const bool DefaultLocked = false ;

    [ Obsolete ( "Use AppConfiguration.Defaults.DeskMinHeightInCm" ) ]
    public const uint DefaultDeskMinHeightInCm = 60 ;

    [ Obsolete ( "Use AppConfiguration.Defaults.DeskMaxHeightInCm" ) ]
    public const uint DefaultDeskMaxHeightInCm = 127 ;

    [ Obsolete ( "Use AppConfiguration.Defaults.AppearanceThemeName" ) ]
    public const string DefaultAppearanceThemeName = "theme_light" ;

    [ Obsolete ( "Use AppConfiguration.Defaults.GlobalHotkeysEnabled" ) ]
    public const bool DefaultGlobalHotkeysEnabled = true ;

    [ Obsolete ( "Use AppConfiguration.Hotkeys.DefaultModifiers" ) ]
    public const string DefaultHotkeyModifiers = "Control, Alt, Shift" ;

    [ Obsolete ( "Use AppConfiguration.Hotkeys.StandingKey" ) ]
    public const string DefaultStandingKey = "Up" ;

    [ Obsolete ( "Use AppConfiguration.Hotkeys.SeatingKey" ) ]
    public const string DefaultSeatingKey = "Down" ;

    [ Obsolete ( "Use AppConfiguration.Hotkeys.Custom1Key" ) ]
    public const string DefaultCustom1Key = "Left" ;

    [ Obsolete ( "Use AppConfiguration.Hotkeys.Custom2Key" ) ]
    public const string DefaultCustom2Key = "Right" ;

    [ Obsolete ( "Use AppConfiguration.Hotkeys.StandingName" ) ]
    public const string DefaultStandingName = "Stand" ;

    [ Obsolete ( "Use AppConfiguration.Hotkeys.SeatingName" ) ]
    public const string DefaultSeatingName = "Sit" ;

    [ Obsolete ( "Use AppConfiguration.Hotkeys.Custom1Name" ) ]
    public const string DefaultCustom1Name = "Custom 1" ;

    [ Obsolete ( "Use AppConfiguration.Hotkeys.Custom2Name" ) ]
    public const string DefaultCustom2Name = "Custom 2" ;

    [ Obsolete ( "Use AppConfiguration.UI.DeskHeightFactor" ) ]
    public const uint DeskHeightFactor = 100 ;

    [ Obsolete ( "Use AppConfiguration.Timeouts.HeightChangeThrottleSeconds" ) ]
    public const int HeightChangeThrottleSeconds = 1 ;

    [ Obsolete ( "Use AppConfiguration.Defaults.NotificationsEnabled" ) ]
    public static readonly bool NotificationsEnabled = true ;

    /// <summary>
    ///     Timeout configuration for various operations
    /// </summary>
    [ Obsolete ( "Use AppConfiguration.Timeouts" ) ]
    public static class Timeouts
    {
        [ Obsolete ( "Use AppConfiguration.Timeouts.InitializationSeconds" ) ]
        public const int InitializationSeconds = 60 ;

        [ Obsolete ( "Use AppConfiguration.Timeouts.StatusBarInfoClearDelaySeconds" ) ]
        public const int StatusBarInfoClearDelaySeconds = 10 ;

        [ Obsolete ( "Use AppConfiguration.Timeouts.SettingsAutoSaveThrottleMilliseconds" ) ]
        public const int SettingsAutoSaveThrottleMilliseconds = 300 ;
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
