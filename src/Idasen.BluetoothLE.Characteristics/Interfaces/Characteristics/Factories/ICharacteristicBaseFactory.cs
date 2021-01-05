using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Factories
{
    public interface ICharacteristicBaseFactory
    {
        T Create < T > ( IDevice device ) ;
    }
}