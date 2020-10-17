using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Factories
{
    public interface IControlFactory
    {
        IControl Create(IDevice device);
    }
}