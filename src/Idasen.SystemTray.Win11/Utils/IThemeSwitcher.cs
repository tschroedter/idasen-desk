namespace Idasen.SystemTray.Win11.Utils ;

public interface IThemeSwitcher
{
    void   ChangeTheme ( string parameter ) ;
    string CurrentThemeName { get ; }
}