using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Factories;
using Idasen.BluetoothLE.Core;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery;
using JetBrains.Annotations;

namespace Idasen.BluetoothLE.Characteristics.Characteristics.Factories
{
    public class ReferenceInputFactory // todo generic factory
        : IReferenceInputFactory
    {
        private readonly ReferenceInput.Factory _factory;

        public ReferenceInputFactory([NotNull] ReferenceInput.Factory factory)
        {
            Guard.ArgumentNotNull(factory,
                                  nameof(factory));

            _factory = factory;
        }

        public IReferenceInput Create([NotNull] IDevice device)
        {
            Guard.ArgumentNotNull(device,
                                  nameof(device));

            return _factory(device);
        }
    }
}