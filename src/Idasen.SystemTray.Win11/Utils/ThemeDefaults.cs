using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Utils ;

public static class ThemeDefaults
{
    public static string GetDefaultThemeName ( )
    {
        try
        {
            var theme = ApplicationThemeManager.GetAppTheme ( ) ;

            return theme is ApplicationTheme.Light or ApplicationTheme.Dark or ApplicationTheme.HighContrast
                       ? theme.ToString ( )
                       : nameof ( ApplicationTheme.Light ) ;
        }
        catch
        {
            // In test or non-UI contexts, fetching the theme can throw. Fall back to a safe default.
            return nameof ( ApplicationTheme.Light ) ;
        }
    }
}