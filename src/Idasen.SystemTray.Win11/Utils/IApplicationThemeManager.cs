using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Utils ;

public interface IApplicationThemeManager
{
    public ApplicationTheme GetAppTheme ( ) ;

    public void Apply ( ApplicationTheme theme ) ;
}