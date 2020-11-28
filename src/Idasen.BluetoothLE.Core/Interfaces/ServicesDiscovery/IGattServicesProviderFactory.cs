using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery
{
    public interface IGattServicesProviderFactory
    {
        IGattServicesProvider Create ( [ NotNull ] IBluetoothLeDeviceWrapper wrapper ) ;
    }
}