using System ;
using Hardcodet.Wpf.TaskbarNotification ;
using Idasen.BluetoothLE.Linak.Interfaces ;

namespace Idasen.SystemTray.Interfaces
{
    public interface ITaskbarIconProvider : IDisposable
    {
        TaskbarIcon NotifyIcon { get ; }
        void        Initialize ( IDesk desk ) ;
    }
}