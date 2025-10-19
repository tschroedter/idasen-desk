using System.Diagnostics.CodeAnalysis ;
using Windows.UI ;
using Windows.UI.ViewManagement ;
using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Utils ;

[ ExcludeFromCodeCoverage ]
public static class ThemeDefaults
{
    public static string GetDefaultThemeName ( )
    {
        try
        {
            var uiSettings = new UISettings ( ) ;

            var isLightTheme = IsColorLight ( uiSettings.GetColorValue ( UIColorType.Background ) ) ;

            // No highcontrast detection yet because it's using WMI query
            // see https://engy.us/blog/2018/10/20/dark-theme-in-wpf/#:~:text=We%E2%80%99ll%20need%20to%20find%20if%20High%20Contrast%20is,a%20value%20from%20this%20enum%3A%20Light%2C%20Dark%2C%20HighContrast
            return isLightTheme
                       ? nameof ( ApplicationTheme.Light )
                       : nameof ( ApplicationTheme.Dark ) ;
        }
        catch
        {
            // In test or non-UI contexts, fetching the theme can throw. Fall back to a safe default.
            return nameof ( ApplicationTheme.Light ) ;
        }
    }

    // From https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/apply-windows-themes?WT.mc_id=DT-MVP-5003978#know-when-dark-mode-is-enabled
    private static bool IsColorLight ( Color clr )
    {
        return 5 * clr.G + 2 * clr.R + clr.B > 8 * 128 ;
    }
}