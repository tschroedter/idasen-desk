using System ;
using Idasen.SystemTray.Interfaces ;
using Idasen.SystemTray.Utils ;

namespace Idasen.SystemTray.Settings;

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