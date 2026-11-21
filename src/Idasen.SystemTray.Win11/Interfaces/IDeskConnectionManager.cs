using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IDeskConnectionManager : IDisposable
{
    bool IsConnected { get ; }

    IDesk ? CurrentDesk { get ; }

    IBluetoothConnectionMonitor ? ConnectionMonitor { get ; }

    [UsedImplicitly]
    event EventHandler ?           Connected ;
    [UsedImplicitly]
    event EventHandler ?           Disconnected ;
    event EventHandler < IDesk > ? DeskReady ;

    Task ConnectAsync ( CancellationToken cancellationToken ) ;
    Task DisconnectAsync ( ) ;
}
