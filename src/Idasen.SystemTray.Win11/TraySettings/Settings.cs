using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.TraySettings;

public class Settings : ISettings
{
    public DeviceSettings DeviceSettings { get ; set ; } = new( ) ;

    public HeightSettings HeightSettings { get ; set ; } = new( ) ;

    public bool NotificationsEnabled { get ; set ; } = Constants.NotificationsEnabled ; // todo duplicated in Device Settings

    public override string ToString ( )
    {
        return $"{nameof ( DeviceSettings )} = {DeviceSettings}, " +
               Environment.NewLine                                 +
               $"{nameof ( HeightSettings )} = {HeightSettings}, " +
               $"{nameof ( NotificationsEnabled )} = {NotificationsEnabled}" ;
    }
}