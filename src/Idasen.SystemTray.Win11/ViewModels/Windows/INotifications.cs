using Autofac ;

namespace Idasen.SystemTray.Win11.ViewModels.Windows ;

public interface INotifications
{
    void Show ( string     title ,
                            string     text ,
                            Visibility visibilityBulbGreen  = Visibility.Hidden ,
                            Visibility visibilityBulbYellow = Visibility.Hidden ,
                            Visibility visibilityBulbRed    = Visibility.Hidden ) ;

    INotifications Initialize ( IContainer container ) ;
}