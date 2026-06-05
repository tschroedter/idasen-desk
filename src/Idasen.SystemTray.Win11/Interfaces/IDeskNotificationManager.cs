using Idasen.SystemTray.Win11.Utils ;
using Wpf.Ui.Controls ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IDeskNotificationManager : IDisposable
{
    IObservable < StatusBarInfo > StatusBarInfoChanged { get ; }

    void Initialize ( NotifyIcon notifyIcon , CancellationToken token ) ;

    void ShowNotification ( string title , string message , InfoBarSeverity severity ) ;

    void ShowStatusUpdate ( uint            height ,
                            string          title ,
                            string          message ,
                            InfoBarSeverity severity ) ;
}
