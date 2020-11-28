using System ;
using System.Threading.Tasks ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers
{
    public interface IGattCharacteristicValueChangedObservables
        : IDisposable
    {
        IObservable < GattCharacteristicValueChangedDetails > ValueChanged { get ; }
        Task                                                  Initialise ( GattCharacteristic characteristic ) ;
    }
}