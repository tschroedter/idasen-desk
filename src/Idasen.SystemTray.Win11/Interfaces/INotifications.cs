using Wpf.Ui.Controls ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface INotifications : IDisposable
{
    INotifications Initialize ( NotifyIcon        notifyIcon ,
                                CancellationToken token ) ;

    void Show ( string          title ,
                string          text ,
                InfoBarSeverity serverity ) ;
}