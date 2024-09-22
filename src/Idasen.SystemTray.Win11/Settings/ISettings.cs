namespace Idasen.SystemTray.Win11.Settings ;

public interface ISettings
{
    DeviceSettings DeviceSettings { get ; set ; }

    HeightSettings HeightSettings { get ; set ; }
    
    bool   NotificationsEnabled    { get ; set ; }
}