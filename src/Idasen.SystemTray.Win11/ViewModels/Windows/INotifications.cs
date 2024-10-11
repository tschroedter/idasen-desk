using Autofac ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.ViewModels.Windows ;

public interface INotifications : IDisposable
{
    void Show ( NotificationParameters parameters ) ;

    INotifications Initialize ( IContainer container , NotifyIcon notifyIcon ) ;

    void Show(string     title,
              string     text,
              Visibility visibilityBulbGreen  = Visibility.Hidden,
              Visibility visibilityBulbYellow = Visibility.Hidden,
              Visibility visibilityBulbRed    = Visibility.Hidden) ;
}