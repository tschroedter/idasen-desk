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

    public static readonly bool NotificationsEnabled = true ;
}