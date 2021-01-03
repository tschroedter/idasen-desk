using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics
{
    public interface ITestCharacteristicBase
        : ICharacteristicBase
    {
        delegate ITestCharacteristicBase Factory ( IDevice device ) ;
    }
}