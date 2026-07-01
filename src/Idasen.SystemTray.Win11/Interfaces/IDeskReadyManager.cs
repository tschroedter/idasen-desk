using Idasen.BluetoothLE.Linak.Interfaces ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IDeskReadyManager : IDisposable
{
    void OnDeskReady ( IDesk desk ) ;
}
