using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Utils ;

public static class ThemeDefaults
{
    public static string GetDefaultThemeName ( )
    {
        var theme = ApplicationThemeManager.GetAppTheme ( ) ;

        return theme is ApplicationTheme.Light or ApplicationTheme.Dark or ApplicationTheme.HighContrast
                   ? theme.ToString ( )
                   : nameof ( ApplicationTheme.Light ) ;
    }
}