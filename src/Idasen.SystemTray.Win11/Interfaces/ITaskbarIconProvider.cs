using Idasen.BluetoothLE.Linak.Interfaces ;
using Serilog ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Interfaces
{
    public interface ITaskbarIconProvider : IDisposable
    {
        void Initialize ( ILogger      logger ,
                          IDesk        desk ,
                          NotifyIcon ? notifyIcon ) ;
    }
}