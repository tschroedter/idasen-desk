namespace Idasen.SystemTray.Win11.Utils ;

public interface IThemeSwitcher
{
    string CurrentThemeName { get ; }
    void   ChangeTheme ( string parameter ) ;
}