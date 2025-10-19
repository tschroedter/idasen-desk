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

    // PayPal donation URL opened when clicking the donate button on the Home page
    public const string DonateUrl = "https://www.paypal.com/donate/?hosted_button_id=KAWJDNVJTD7SJ" ;

    public static readonly bool NotificationsEnabled = true ;
}