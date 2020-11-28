using Windows.Devices.Bluetooth.Advertisement ;
using Idasen.BluetoothLE.Core.DevicesDiscovery ;

namespace Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery
{
    public interface IStatusMapper
    {
        Status Map ( BluetoothLEAdvertisementWatcherStatus status ) ;
    }
}