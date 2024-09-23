using Hardcodet.Wpf.TaskbarNotification ;

namespace Idasen.SystemTray.Win11
{
    public interface IDynamicIconCreator
    {
        void Update ( TaskbarIcon taskbarIcon ,
                      int         height ) ;
    }
}