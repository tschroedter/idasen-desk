using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Factories;
using Idasen.BluetoothLE.Core;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery;
using JetBrains.Annotations;

namespace Idasen.BluetoothLE.Characteristics.Characteristics.Factories
{
    public class ControlFactory // todo generic factory
        : IControlFactory
    {
        private readonly Control.Factory _factory;

        public ControlFactory([NotNull] Control.Factory factory)
        {
            Guard.ArgumentNotNull(factory,
                                  nameof(factory));

            _factory = factory;
        }

        public IControl Create([NotNull] IDevice device)
        {
            Guard.ArgumentNotNull(device,
                                  nameof(device));

            return _factory(device);
        }
    }
}