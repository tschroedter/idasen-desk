using Autofac ;
using Wpf.Ui.Controls ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface INotifications : IDisposable
{
    INotifications Initialize ( IContainer container , NotifyIcon notifyIcon ) ;

    void Show ( string          title ,
                string          text ,
                InfoBarSeverity serverity ) ;
}