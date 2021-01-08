using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics
{
    public interface ICharacteristicBaseFactory
    {
        T Create < T > ( IDevice device ) ;
    }
}