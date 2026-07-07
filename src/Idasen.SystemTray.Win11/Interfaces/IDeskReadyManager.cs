using Idasen.BluetoothLE.Linak.Interfaces ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IDeskReadyManager : IDisposable
{
    void OnDeskReady ( IDesk     desk ) ;
    void Initialize ( NotifyIcon notifyIcon ) ;    
}
