using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Utils ;

public class ThemeSwitcher ( IApplicationThemeManager themeManager ) : IThemeSwitcher
{
    public void ChangeTheme ( string parameter )
    {
        parameter = parameter.Trim ( ).ToLowerInvariant ( ) ;

        switch ( parameter )
        {
            case "light" :
            case "theme_light" :
                if ( themeManager.GetAppTheme ( ) == ApplicationTheme.Light )
                    break ;

                themeManager.Apply ( ApplicationTheme.Light ) ;

                break ;

            case "dark" :
            case "theme_dark" :
                if ( themeManager.GetAppTheme ( ) == ApplicationTheme.Dark )
                    break ;

                themeManager.Apply ( ApplicationTheme.Dark ) ;

                break ;

            case "highcontrast" :
            case "high_contrast" :
            case "theme_high_contrast" :
                if ( themeManager.GetAppTheme ( ) == ApplicationTheme.HighContrast )
                    break ;

                themeManager.Apply ( ApplicationTheme.HighContrast ) ;

                break ;

            default :
                if ( themeManager.GetAppTheme ( ) == ApplicationTheme.Unknown )
                    break ;

                themeManager.Apply ( ApplicationTheme.Unknown ) ;

                break ;
        }
    }

    public string CurrentThemeName => themeManager.GetAppTheme ( )
                                                  .ToString ( ) ;
}