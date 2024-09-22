using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.Settings;

public class Settings : ISettings
{
    public DeviceSettings DeviceSettings { get ; set ; } = new DeviceSettings ( ) ;

    public HeightSettings HeightSettings { get ; set ; } = new HeightSettings ( ) ;

    public bool NotificationsEnabled { get ; set ; } = Constants.NotificationsEnabled ;

    public override string ToString ( )
    {
        return $"{nameof ( DeviceSettings )} = {DeviceSettings}, " +
               Environment.NewLine                                 +
               $"{nameof ( HeightSettings )} = {HeightSettings}, " +
               $"{nameof ( NotificationsEnabled )} = {NotificationsEnabled}" ;
    }
}