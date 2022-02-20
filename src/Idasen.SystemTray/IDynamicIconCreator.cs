using Hardcodet.Wpf.TaskbarNotification ;

namespace Idasen.SystemTray
{
    public interface IDynamicIconCreator
    {
        void Update ( TaskbarIcon taskbarIcon ,
                      int         height ) ;
    }
}