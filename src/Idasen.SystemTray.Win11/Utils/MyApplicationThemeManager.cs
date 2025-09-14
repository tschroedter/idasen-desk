using System.Diagnostics.CodeAnalysis ;
using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Utils ;

[ ExcludeFromCodeCoverage ]
public class MyApplicationThemeManager : IApplicationThemeManager
{
    public ApplicationTheme GetAppTheme ( )
    {
        return ApplicationThemeManager.GetAppTheme ( ) ;
    }

    public void Apply ( ApplicationTheme theme )
    {
        ApplicationThemeManager.Apply ( theme ) ;
    }
}