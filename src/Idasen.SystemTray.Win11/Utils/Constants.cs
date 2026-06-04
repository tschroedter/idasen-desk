namespace Idasen.SystemTray.Win11.Utils ;

public static class Constants
{
    public const string ApplicationName  = "Idasen.SystemTray" ;
    public const string SettingsFileName = "Settings.json" ;

    public const uint   DefaultHeightStandingInCm      = 120 ;
    public const uint   DefaultHeightSeatingInCm       = 65 ;
    public const string DefaultDeviceName              = "Desk" ;
    public const ulong  DefaultDeviceAddress           = 250635178951455u ;
    public const uint   DefaultDeviceMonitoringTimeout = 600 ; // in seconds
    public const bool   DefaultLocked                  = false ;
    public const uint   DefaultDeskMinHeightInCm       = 60 ;
    public const uint   DefaultDeskMaxHeightInCm       = 127 ;
    public const string DefaultAppearanceThemeName     = "theme_light" ;

    public const bool   DefaultGlobalHotkeysEnabled = true ;
    public const string DefaultHotkeyModifiers      = "Control, Alt, Shift" ;
    public const string DefaultStandingKey          = "Up" ;
    public const string DefaultSeatingKey           = "Down" ;
    public const string DefaultCustom1Key           = "Left" ;
    public const string DefaultCustom2Key           = "Right" ;

    public const string DefaultStandingName         = "Stand" ;
    public const string DefaultSeatingName          = "Sit" ;
    public const string DefaultCustom1Name          = "Custom 1" ;
    public const string DefaultCustom2Name          = "Custom 2" ;

    public static readonly bool NotificationsEnabled = true ;

    /// <summary>
    ///     Conversion factor between desk height in millimeters and centimeters.
    ///     Desk hardware reports height in millimeters, UI displays in centimeters.
    /// </summary>
    public const uint DeskHeightFactor = 100 ;

    /// <summary>
    ///     Throttle duration in seconds for desk height change notifications.
    ///     Prevents excessive updates during continuous desk movement.
    /// </summary>
    public const int HeightChangeThrottleSeconds = 1 ;
}