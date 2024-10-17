using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class Settings : ISettings
{
    public DeviceSettings DeviceSettings { get ; set ; } = new( ) ;

    public HeightSettings HeightSettings { get ; set ; } = new( ) ;

    public override string ToString ( )
    {
        return $"{nameof ( DeviceSettings )} = {DeviceSettings}, " +
               $"{nameof ( HeightSettings )} = {HeightSettings}" ;
    }
}