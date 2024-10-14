using Autofac ;
using Idasen.SystemTray.Win11.Utils ;
using JetBrains.Annotations ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface INotifications : IDisposable
{
    [ UsedImplicitly ]
    void Show ( NotificationParameters parameters ) ; // todo future usage

    INotifications Initialize ( IContainer container , NotifyIcon notifyIcon ) ;

    void Show ( string     title ,
                string     text ,
                Visibility visibilityBulbGreen  = Visibility.Hidden ,
                Visibility visibilityBulbYellow = Visibility.Hidden ,
                Visibility visibilityBulbRed    = Visibility.Hidden ) ;
}