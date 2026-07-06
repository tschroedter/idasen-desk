using Idasen.BluetoothLE.Linak.Interfaces ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IDeskConnectionManager : IDisposable
{
    bool IsConnected { get ; }

    IDesk ? CurrentDesk { get ; }

    IBluetoothConnectionMonitor ? ConnectionMonitor { get ; }

    event EventHandler ?           Connected ;
    event EventHandler ?           Disconnected ;
    event EventHandler < IDesk > ? DeskReady ;

    Task ConnectAsync ( CancellationToken cancellationToken ) ;
    Task DisconnectAsync ( ) ;
}
