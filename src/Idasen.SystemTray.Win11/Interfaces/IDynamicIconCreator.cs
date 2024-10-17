using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IDynamicIconCreator
{
    void Update ( NotifyIcon taskbarIcon ,
                  int        height ) ;
}