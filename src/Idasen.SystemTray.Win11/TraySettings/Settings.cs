using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class Settings : ISettings
{
    public DeviceSettings DeviceSettings { get ; set ; } = new( ) ;

    public HeightSettings HeightSettings { get ; set ; } = new( ) ;

    public AppearanceSettings AppearanceSettings { get ; set ; } =
        new( ) { ThemeName = ThemeDefaults.GetDefaultThemeName ( ) } ;

    public override string ToString ( )
    {
        return $"{nameof ( DeviceSettings )} = {DeviceSettings}, " +
               $"{nameof ( HeightSettings )} = {HeightSettings}, " +
               $"{nameof ( AppearanceSettings )} = {AppearanceSettings}" ;
    }
}