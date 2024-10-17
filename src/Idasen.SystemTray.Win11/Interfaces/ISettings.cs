using Idasen.SystemTray.Win11.TraySettings ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface ISettings
{
    DeviceSettings DeviceSettings { get ; set ; }

    HeightSettings HeightSettings { get ; set ; }
}