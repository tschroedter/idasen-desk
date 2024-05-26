using Idasen.SystemTray.Interfaces ;

public interface ISettings
{
    DeviceSettings DeviceSettings { get ; set ; }

    HeightSettings HeightSettings { get ; set ; }
    
    bool   NotificationsEnabled    { get ; set ; }
}