using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Factories
{
    public interface IDpgFactory
    {
        IDpg Create(IDevice device);
    }
}