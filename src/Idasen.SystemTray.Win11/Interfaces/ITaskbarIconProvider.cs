using Hardcodet.Wpf.TaskbarNotification ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Interfaces
{
    public interface ITaskbarIconProvider : IDisposable
    {
        void Initialize ( ILogger       logger ,
                          IDesk         desk ,
                          TaskbarIcon ? notifyIcon ) ;
    }
}